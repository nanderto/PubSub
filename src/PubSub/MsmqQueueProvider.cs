namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Messaging;
    using System.Text;
    using System.Threading;
    using System.Transactions;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Phantom.PubSub;

    /// <summary>
    /// This Queue provide is specific for MSMQ queues. It encapsulates all Queue operations
    /// Alternative Queue mechanisms may be substituted. Implement IQueueProvider to substitue a new queue type
    /// </summary>
    /// <typeparam name="T">Type of Message</typeparam>
    public class MsmqQueueProvider<T> : IQueueProvider<T>, IDisposable // , servicepipeline.IQueueProvider<T>
    {
        private MessageQueue msgQ;
        
        private string queueName = string.Empty;

        public MsmqQueueProvider()
        {
            this.Name = CleanupName(typeof(T).ToString());
        }

        public delegate void MessageSentEventHandler(object sender, MessageSentEventArgs e);

        public delegate void MessageHandlingInitiated(T message, string MessageID);

        public List<ISubscriber<T>> Subscribers { get; set; }

        public virtual string ConfigureQueue(string queueName)
        {
            this.Name = CleanupName(queueName);
            this.queueName = @".\private$\" + this.Name;
            
            string queuename = this.Name;
            queuename = @".\private$\" + queuename;
            
            if (!MessageQueue.Exists(queuename))
            {
                this.msgQ = MessageQueue.Create(queuename, true);
            }
            else
            {
                this.msgQ = new MessageQueue(queuename);
            }

            // create a poinson message queue
            queuename = queuename + "PoisonMessages";
            if (!MessageQueue.Exists(queuename))
            {
                var poisonMsgQ = MessageQueue.Create(queuename, true);
                poisonMsgQ.Dispose();
            }

            return this.Name;
        }

        public virtual string PutMessage(T message)
        {
            if (message == null)
                throw new ArgumentNullException("Message is null for Queue name: " + this.queueName);

            string result = string.Empty;
            
            if (CheckQueueConfigured())
            {
                using (MessageQueue msgQueue = new MessageQueue(this.queueName))
                {
                    using (MessageQueueTransaction msgTx = new MessageQueueTransaction())
                    {                      
                        Message recoverableMessage = null;
                        msgTx.Begin();
                        try
                        {
                            recoverableMessage = new Message();
                            recoverableMessage.Body = message;
                            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
                            recoverableMessage.Recoverable = true;

                            msgQueue.Send(recoverableMessage, msgTx);
                            ////this.msgQ.Send(recoverableMessage, msgTx); //whats up here??? this will teach me for taking time off. not sure whuc I shuold use
                            msgTx.Commit();
                            result = recoverableMessage.Id;
                            //recoverableMessage.Dispose();
                        }
                        catch (Exception)
                        {
                            msgTx.Abort();
                            ////recoverableMessage.Dispose();
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
            }

            return result; 
        }

        public void OnMessageSentEventHandler(MessageSentEventArgs e)
        {
        }

        public string Name { get; set; }

        public void SetUpWatchQueue(IQueueProvider<T> queueProvider)
        {
            this.msgQ = FindQueue(queueProvider.Name);
        }

        /// <summary>
        /// Reads queue without removing message
        /// </summary>
        /// <returns>Message of type T</returns>
        public T ReadQueue(out string messageId)
        {
            messageId = string.Empty;
            if (CheckQueueConfigured())
            {
                using (MessageQueue msgQueue = new MessageQueue(this.queueName))
                {
                    try
                    {
                        // this should be set to 0 why block thread at all if ther is no message
                        Message m = msgQueue.Peek((new TimeSpan(1000)));
                        m.Formatter = new BinaryMessageFormatter();
                        messageId = m.Id;
                        return (T)m.Body;
                    }
                    catch (Exception ex)
                    {

                        System.Diagnostics.Trace.WriteLine("Failed to PEEK: " + messageId + " " + ex.ToString());
                    }
                }
            }
            
            return default(T);
        }

        public bool RemoveFromQueue(string MessageId)
        {
            Message M = null;
            ////if (CheckQueueConfigured())
            ////{
                using (MessageQueue msgQueue = FindQueue(this.Name))
                {
                    try
                    {
                        Counter.Increment(3);
                        System.Diagnostics.Debug.WriteLine("Calling RemoveFromQueue :: The MessageId is: " + MessageId + " Counter: " + Counter.Subscriber(3));
                        M = msgQueue.ReceiveById(MessageId);                       
                    }
                    catch (MessageQueueException ex)
                    {
                        System.Diagnostics.Trace.WriteLine("Failed to remove: " + MessageId + " " + ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine("Failed to remove: " + MessageId + " " + ex.ToString());
                    }
                }

            if (M == null)
            {
                return false;
            }
            else
            {
                M = null;
                return true;
            }
        }

        /// <summary>
        /// Processes all messages in Queue
        /// </summary>
        public void ProcessQueueAsBatch(MessageHandlingInitiated messageHandlingInitiated)
        {
            string Id = string.Empty;
            ////if (CheckQueueConfigured())
            ////{
            using (MessageQueue msgQueue = this.FindQueue(this.Name))
            {
                try
                {
                    if (!IsQueueEmpty(msgQueue))
                    {
                        Counter.Increment(4);
                        LogEntry logEntry = new LogEntry();
                        logEntry.Message = "Calling GetAllMessages::Number of times called " + Counter.Subscriber(4);
                        Logger.Write(logEntry);
                        Message[] messages = new Message[] { };

                        messages = msgQueue.GetAllMessages();
                        int numberOfMessages = messages.Length;
                        logEntry.Message = "Number of messages returned:  " + numberOfMessages.ToString();
                        Logger.Write(logEntry);
                        int i = 0;
                        foreach (Message m in messages)
                        {
                            //lets get rid of throtteling
                           // if (i > 20) break;
                            try
                            {
                                i++;
                                m.Formatter = new BinaryMessageFormatter();
                                var message = (T)m.Body;
                                Id = m.Id;

                                messageHandlingInitiated((T)m.Body, m.Id);
                            }
                            catch (MessageQueueException ex)
                            {
                                // log the error but just let the application continue.
                                // this will get retried. 
                                // need to handle bad messages that cause this to fail 
                                // all other exceptions allowed to traverse back up stack
                                ///todo do this
                                System.Diagnostics.Trace.WriteLine("Failed to read queue: dending to poison Queue" + ex.ToString());
                                HandlePoisonMessage(Id);
                            }
                            catch (Exception ex)
                            {
                                // these catches will ensure next message is processed
                                System.Diagnostics.Trace.WriteLine("Swallowing General exception letting process continue" + ex.ToString());
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
                    ///todo do this
                    System.Diagnostics.Trace.WriteLine("Message Queue Exception: " + ex.ToString());
                    HandlePoisonMessage(Id);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Swallowing General exception letting process continue" + ex.ToString());
                }
            }
        }

        public void HandlePoisonMessage(string messageId)
        {
            string PoisonQueueName = this.Name + "PoisonMessages";
           
            Message message;
      
            System.Diagnostics.Trace.WriteLine("HandlePoisonMessage with look up id: " + messageId + " to poison queue: " + PoisonQueueName);
            
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
                                using (MessageQueue poisonMessageQueue = this.FindQueue(PoisonQueueName))
                                {
                                   poisonMessageQueue.Send(message, System.Messaging.MessageQueueTransactionType.Automatic);                         
                                }
                                 
                                // complete transaction scope
                                transactionScope.Complete();

                                System.Diagnostics.Trace.WriteLine("Moved poisoned message with look up id: " + messageId + " to poison queue: " + PoisonQueueName);
                                break;
                            }
                        }
                        catch (InvalidOperationException)
                        {
                            // Code for the case when the message may still not be available in the queue because of a race condition in transaction or 
                            // another node in the farm may actually have taken the message.
                            if (retryCount < 3)
                            {
                                System.Diagnostics.Trace.WriteLine("Trying to move poison message but message is not available, sleeping for 10 seconds before retrying");
                                Thread.Sleep(TimeSpan.FromSeconds(10));
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLine("Giving up on trying to move the message try lowering priority");
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
                                    System.Diagnostics.Trace.WriteLine("Error Policy" + ex.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsQueueEmpty()
        {
            return IsQueueEmpty(this.msgQ);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MsmqQueueProvider()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.msgQ.Dispose();
            }
        }

        #endregion

        private bool IsQueueEmpty(MessageQueue Queue)
        {
            
            bool isQueueEmpty = false;
            try
            {
                //Counter.Increment(5);
                //LogEntry logEntry = new LogEntry();
                //logEntry.Message = "Calling peek to check if anything in queue: " + Counter.Subscriber(5);
                //Logger.Write(logEntry);
                Queue.Peek(new TimeSpan(0));
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

        private static string CleanupName(string dirtyname)
        {
            return dirtyname.ToString().Replace("{", "").Replace("}", "").Replace("_", "").Replace(".", "");
        }

        private bool CheckQueueConfigured()
        {
            if (this.queueName == string.Empty)
            {
                if (this.Name == string.Empty)
                {
                    this.Name = CleanupName(typeof(T).ToString());
                }

                this.queueName = @".\private$\" + this.Name;
            }

            if (!MessageQueue.Exists(this.queueName))
            {
                throw new Exception("Queue: " + this.queueName + " not configured");
            }

            return true;
        }

        private MessageQueue FindQueue(string queueName)
        {           
            queueName = @".\private$\" + queueName;
            if (!MessageQueue.Exists(queueName))
            {
                throw new Exception("Queue: " + queueName + "does not exist");
            }
            else
            {
                return new MessageQueue(queueName);
            }
        }
    }
}

