namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Messaging;
    using System.Text;
    using System.Threading;
    using System.Transactions;
    using Phantom.PubSub;

    /// <summary>
    /// This Store provider is specific for MSMQ queues. It encapsulates all storage operations
    /// Alternative storage mechanisms may be substituted. Implement IStoreProvider to substitute a new queue type
    /// </summary>
    /// <typeparam name="T">Type of Message</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Msmq", Justification = "Spelling is correct")]
    public class MsmqStoreProvider<T> : IStoreProvider<T>
    {
        private static readonly object SyncLock = new object();
        private static volatile bool isQueueConfigured = false;
        private string longStoreName = string.Empty;

        public MsmqStoreProvider()
        {
            this.Name = CleanupName(typeof(T).ToString());
            if (!isQueueConfigured)
            {
                lock (SyncLock)
                {
                    isQueueConfigured = this.ConfigureStore(this.Name, StoreTransactionOption.SupportTransactions);
                }
            }
        }

        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Want to swallow all exceptions and allow code to continuing to execute")]
        public string PutMessage(MessagePacket<T> message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("Message is null for Queue name: " + this.Name);
            }

            if (string.IsNullOrEmpty(this.Name))
            {
                throw new ArgumentNullException("Queue provider name is null for Queue name: " + this.Name);
            }

            Counter.Increment(17);
           //// Trace.WriteLine("Putting in Queue: " + message.Id);

            string result = string.Empty;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (MessageQueue msgQueue = new MessageQueue(@".\private$\" + this.Name))
                {
                    Message recoverableMessage = null;

                    try
                    {
                        recoverableMessage = new Message();
                        recoverableMessage.Body = message;
                        recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
                        recoverableMessage.Recoverable = true;

                        msgQueue.Send(recoverableMessage);

                        scope.Complete();
                        result = recoverableMessage.Id;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        if (recoverableMessage != null)
                        {
                            recoverableMessage.Dispose();
                        }
                    }
                }
            }

            return result;
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Want to swallow all exceptions and allow code to continuing to execute")]
        public bool RemoveFromStorage(string messageId)
        {
            Message m = null;
            //// if (CheckQueueConfigured())
            //// {
            using (MessageQueue msgQueue = FindQueue(this.Name))
            {
                try
                {
                    Counter.Increment(5);
                    ////Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub", "Phantom.PubSub")).TraceInfo, "Calling RemoveFromQueue :: The MessageId is: " + messageId + " Counter: " + Counter.Subscriber(3));
                    ////Trace.WriteLine("Just about to remove subscription from Queue: " + messageId);
                    m = msgQueue.ReceiveById(messageId);
                    ////Trace.WriteLine("Just removed subscription from Queue: " + messageId);
                }
                catch (MessageQueueException ex)
                {
                    Counter.Increment(9);
                    Trace.WriteLine(new InvalidOperationException("Message Queue Exception Failed to remove: " + messageId, ex));
                }
                catch (Exception ex)
                {
                    Counter.Increment(9);
                    Trace.WriteLine(new InvalidOperationException("general Exception Failed to remove: " + messageId, ex));
                }
            }

            if (m == null)
            {
                return false;
            }
            else
            {
                m = null;
                return true;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Want to swallow all exceptions and allow code to continuing to execute")]
        public bool CheckItsStillInTheStore(string messageId)
        {
            Message m = null;
            using (MessageQueue msgQueue = FindQueue(this.Name))
            {
                try
                {
                    Counter.Increment(10);
                    //// Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub", "Phantom.PubSub")).TraceInfo, "Calling CheckItsStillInTheQueue :: The MessageId is: " + MessageId + " Counter: " + Counter.Subscriber(3));
                    m = msgQueue.PeekById(messageId);
                }
                catch (InvalidOperationException)
                {
                    Counter.Increment(11);
                    return false;
                }
                catch (MessageQueueException ex)
                {
                    Trace.WriteLine(new InvalidOperationException("Message Queue Exception Peek Failed: " + messageId, ex));
                }
                catch (Exception ex)
                {
                    Counter.Increment(11);
                    Trace.WriteLine(new InvalidOperationException("General Exception Peek Failed: " + messageId, ex));
                    }
                }

            if (m == null)
            {
                return false;
            }
            else
            {
                m = null;
                return true;
            }
        }

        /// <summary>
        /// Processes all messages in Queue as a batch
        /// </summary>
        /// <param name="messageHandlingInitiated">callback to handle individual messages</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Want to swallow all exceptions and allow code to continuing to execute")]
        public void ProcessStoreAsBatch(Func<MessagePacket<T>, string, bool> messageHandlingInitiated)
        {
            if (messageHandlingInitiated == null)
            {
                throw new ArgumentNullException("messageHandlingInitiated");
            }

            string id = string.Empty;
            using (MessageQueue msgQueue = FindQueue(this.Name))
            {
                try
                {
                    if (!MsmqStoreProvider<T>.IsQueueEmpty(msgQueue))
                    {
                        //// Counter.Increment(3);
                        Message[] messages = new Message[] { };

                        messages = msgQueue.GetAllMessages();
                        ////Trace.WriteLine("Number of messages returned:  " + numberOfMessages.ToString(CultureInfo.InvariantCulture));
 
                        int i = 0;
                        foreach (Message m in messages)
                        {
                            try
                            {
                                i++;
                                m.Formatter = new BinaryMessageFormatter();
                                MessagePacket<T> messagePacket = (MessagePacket<T>)m.Body;
                                messageHandlingInitiated(messagePacket, m.Id);
                            }
                            catch (MessageQueueException ex)
                            {
                                // log the error but just let the application continue.
                                // this will get retried. 
                                // need to handle bad messages that cause this to fail 
                                // all other exceptions allowed to traverse back up stack
                                // todo do this
                                Counter.Increment(6);
                                Trace.WriteLine(ex); 
                                this.HandlePoisonMessage(id);
                            }
                            catch (Exception ex)
                            {
                                // these catches will ensure next message is processed
                                Counter.Increment(6);
                                Trace.WriteLine(ex);
                            }
                        }
                    }
                }
                catch (MessageQueueException ex)
                {
                    // log the error but just let the application continue.
                    // this will get retried. 
                    // need to handle bad messages that cause this to fail 
                    // all other exceptions allowed to traverse back up stack
                    // todo do this
                    Trace.WriteLine(ex);
                    this.HandlePoisonMessage(id);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }

        public void HandlePoisonMessage(string messageId)
        {
            string poisonQueueName = this.Name + "PoisonMessages";
           
            Message message;
            Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub", "Phantom.PubSub")).TraceInfo, "HandlePoisonMessage with look up id: " + messageId + " to poison queue: " + poisonQueueName);
            
            // Use a new transaction scope to remove the message from the main application queue and add it to the poison queue.
            // The poison message service processes messages from the poison queue.
            using (MessageQueue messageQueue = FindQueue(this.Name))
            {
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    int retryCount = 0;
                    while (retryCount < 3)
                    {
                        retryCount++;
                        try
                        {
                            // Look up the poison message using the look up id.
                            message = messageQueue.ReceiveById(messageId);
                            if (message != null)
                            {
                                // Send the message to the poison message queue.
                                using (MessageQueue poisonMessageQueue = FindQueue(poisonQueueName))
                                {
                                   poisonMessageQueue.Send(message, System.Messaging.MessageQueueTransactionType.Automatic);                         
                                }
                                 
                                // complete transaction scope
                                transactionScope.Complete();
                                Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub", "Phantom.PubSub")).TraceInfo, "Moved poisoned message with look up id: " + messageId + " to poison queue: " + poisonQueueName);
                                
                                break;
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // Code for the case when the message may still not be available in the queue because of a race condition in transaction or 
                            // another node in the farm may actually have taken the message.
                            if (retryCount < 3)
                            {
                                Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub", "Phantom.PubSub")).TraceInfo, "Trying to move poison message but message is not available, sleeping for 10 seconds before retrying");

                                Thread.Sleep(TimeSpan.FromSeconds(10));
                            }
                            else
                            {
                                Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub", "Phantom.PubSub")).TraceInfo, "Giving up on trying to move the message try lowering priority"); 

                                try
                                {
                                    // ToDo need to test this - lower priority or move to dead letter queue
                                    // Look up the poison message using the look up id.
                                    message = messageQueue.PeekById(messageId);
                                    if (message != null)
                                    {
                                        message.Priority = MessagePriority.Lowest;
                                    }
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Trace.WriteLine(ex);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool ConfigureStore(string storeName, StoreTransactionOption storeTransactionOption)
        {
            this.Name = CleanupName(storeName);
            this.longStoreName = @".\private$\" + this.Name;
            
            if (!MessageQueue.Exists(this.longStoreName))
            {
                MessageQueue msgQ = null;
                if (storeTransactionOption == StoreTransactionOption.SupportTransactions)
                {
                    msgQ = MessageQueue.Create(this.longStoreName, true);
                    msgQ.Dispose();
                }
                else
                {
                    msgQ = MessageQueue.Create(this.longStoreName);
                    msgQ.Dispose();
                }
            }

            // create a poison message queue
            string queuename = @".\private$\" + this.Name + "PoisonMessages";
            if (!MessageQueue.Exists(queuename))
            {
                var poisonMsgQ = MessageQueue.Create(queuename, true);
                poisonMsgQ.Dispose();
            }

            return true;
        }

        private static string CleanupName(string dirtyname)
        {
            return dirtyname.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);
        }
            
        private static bool IsQueueEmpty(MessageQueue queue)
        {
            bool isQueueEmpty = false;
            try
            {
                Counter.Increment(4);
                queue.Peek(new TimeSpan(0));
                isQueueEmpty = false;
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    isQueueEmpty = true;
                }
                else
                {
                    throw;
                }
            }

            return isQueueEmpty;
        }

        private static MessageQueue FindQueue(string queueName)
        {           
            queueName = @".\private$\" + queueName;
            if (!MessageQueue.Exists(queueName))
            {
                throw new InvalidOperationException("Queue: " + queueName + "does not exist");
            }
            else
            {
                return new MessageQueue(queueName);
            }
        }

        private bool CheckQueueConfigured()
        {
            if (string.IsNullOrEmpty(this.longStoreName))
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    this.Name = CleanupName(typeof(T).ToString());
                }

                this.longStoreName = @".\private$\" + this.Name;
            }

            if (!MessageQueue.Exists(this.longStoreName))
            {
                throw new InvalidOperationException("Queue: " + this.longStoreName + " not configured");
            }

            return true;
        }
    }
}
