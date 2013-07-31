//-----------------------------------------------------------------------
// <copyright file="PublishSubscribeChannel.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Transactions;
    using Phantom.PubSub;

    /// <summary>
    /// PublishSubscribeChannel will accepts new messages puts them on a queue and fire calls to all subscribers
    /// </summary>
    /// <typeparam name="T">The Type that you wish tp publish each type requires its own implementation</typeparam>
    public class PublishSubscribeChannel<T> : IPublishSubscribeChannel<T>
    {                
        private Dictionary<string, Tuple<string, Type, TimeSpan>> subscriberInfos = new Dictionary<string, Tuple<string, Type, TimeSpan>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishSubscribeChannel{T}" /> class.
        /// </summary>
        public PublishSubscribeChannel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishSubscribeChannel{T}" /> class.
        /// </summary>
        /// <param name="queueProvider">The queue provider.</param>
        public PublishSubscribeChannel(IStoreProvider<T> queueProvider)
        {
            this.StorageProvider = queueProvider;
        }

        public IStoreProvider<T> StorageProvider { get; set; }

        public int Count
        {
            get
            {
                return this.StorageProvider.GetMessageCount();
            }
        }

        /// <summary>
        /// Publishes a message first by placing on a durable Queue, then passing the message to a new thread for processing.
        /// Returns as soon as the message is succesfully on the Queue.
        /// This component does not read the message from the Queue to start processing. It takes the same message that is added to the Queue and starts processing
        /// as soon as it is guaranteed to be on the queue.
        /// It will only throw exceptions from the AutoConfiguration
        /// </summary>
        /// <param name="message">The message of the type specified to process</param>
        public void PublishMessage(T message)
        {
            if (!BatchProcessor<T>.IsConfigured)
            {
                BatchProcessor<T>.ConfigureWithPubSubChannel(this);
            }

            if (this.subscriberInfos.Count == 0)
            {
                foreach (var item in AutoConfig<T>.SubscriberInfos)
                {
                    this.subscriberInfos.Add(item.Item1, new Tuple<string, Type, TimeSpan>(item.Item1, item.Item2, item.Item3));
                }
            }

            List<ISubscriberMetadata> metadatalist = new List<ISubscriberMetadata>();

            foreach (var item in this.subscriberInfos)
            {
                var subscribermetadata = new SubscriberMetadata()
                {
                    Name = item.Key, 
                    TimeToExpire = item.Value.Item3,
                    StartTime = DateTime.Now
                };
                metadatalist.Add(subscribermetadata);
            }

            var messageForQueue = new MessagePacket<T>(message, metadatalist);

            string result = this.StorageProvider.PutMessage(messageForQueue);
            Task.Factory.StartNew(() => this.HandleMessageForFirstTime(messageForQueue, result));
        }

        /// <summary>
        /// Batch processing alternative, this is used as part of the clean up process for Messages that expire prior to being handled. 
        /// </summary>
        public void ProcessBatch()
        {
            this.StorageProvider.ProcessStoreAsBatch(this.HandleMessageForBatchProcessing);
        }

        /// <summary>
        /// This method receives a single message and processes all of the subscribers for each individual message. The subscribers are processed in parallel
        /// to ensure that one long running process will not impact the others.
        /// When all subscribers have indicate they have completed a flag is set to indicate that this message has completed publishing, and it is removed 
        /// from the Queue
        /// This method uses the Parallel.ForEach method, so Visual Studio parallel debugging can be used.
        /// It will not return a exception
        /// </summary>
        /// <param name="messagePacket">THe message being sent inside of the messagepacket wrapper</param>
        /// <param name="messageId">ID of message generated by the queue in mechanism</param>
        /// <returns>True on success</returns>
        public bool HandleMessageForFirstTime(MessagePacket<T> messagePacket, string messageId)
        {
            if (messagePacket == null)
            {
                throw new ArgumentNullException("messagePacket");
            }

            if (string.IsNullOrEmpty(messageId))
            {
                throw new ArgumentNullException("messagePacket");
            }

            var subscribersForThisMessage = this.GetSubscriptions();

            this.RunSubscriptions(messagePacket, messageId, subscribersForThisMessage);
            return true;
        }

        /// <summary>
        /// Handles the message for batch processing.
        /// This code is called from the batch processor, which means that it has queried the store for all messages, and sent them to this method
        /// they may or may not have finished processing, if they have then the metadata will have been updated (or updated for those subscribers that have completed or failed)
        /// If they have not finished processing then the metadat will still reflect what was the state of the subscription at the time that the 
        /// message was first saved to the store.
        /// </summary>
        /// <param name="messagePacket">The message packet.</param>
        /// <param name="messageId">The message id.</param>
        /// <returns>Always returns true - need to change to void</returns>
        /// <exception cref="System.ArgumentNullException">Argument Null Exception</exception>
        public bool HandleMessageForBatchProcessing(MessagePacket<T> messagePacket, string messageId)
        {
            if (messagePacket == null)
            {
                throw new ArgumentNullException("messagePacket");
            }

            if (string.IsNullOrEmpty(messageId))
            {
                throw new ArgumentNullException("messageId");
            }

            var runnableSubscribersCollection = new SubscribersCollection<T>();
            foreach (var item in messagePacket.SubscriberMetadataList)
            {
                if (item.CanProcess())
                {
                    string subscriptionId = GetSubscriptionId(messageId, item.Name);
                    var subscriberInfo = this.subscriberInfos.FirstOrDefault(si => si.Key == item.Name);
                    var newSubscription = (ISubscriber<T>)Activator.CreateInstance(subscriberInfo.Value.Item2);
                    newSubscription.Name = subscriberInfo.Value.Item1;
                    newSubscription.TimeToExpire = subscriberInfo.Value.Item3;
                    newSubscription.Id = subscriptionId;
                    newSubscription.AbortCount = item.RetryCount;
                    newSubscription.Aborted = false;
                    newSubscription.MessageId = messageId;
                    newSubscription.StartTime = DateTime.Now;
                    runnableSubscribersCollection.Add(newSubscription); 
                }
            }

            this.RunSubscriptions(messagePacket, messageId, runnableSubscribersCollection);
            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Its Appropriate")]
        public SubscribersCollection<T> GetSubscriptions()
        {
            if (this.subscriberInfos == null || this.subscriberInfos.Count == 0)
            {
                throw new InvalidOperationException("There are no subscribers set up for this channel");
            }

            var subscribers = new SubscribersCollection<T>();
            foreach (var item in this.subscriberInfos)
            {
                ISubscriber<T> subscriber = (ISubscriber<T>)Activator.CreateInstance(item.Value.Item2);
                subscriber.Name = item.Value.Item1;
                subscriber.TimeToExpire = item.Value.Item3;
                subscribers.Add(subscriber);
            }

            return subscribers;
        }

        public ISubscriberInfo<T> AddSubscriberType(Type type)
        {
            return new SubscriberInfo<T>(type, this);
        }

        public IPublishSubscribeChannel<T> AddSubscriberInfo(Tuple<string, Type, TimeSpan> tuple)
        {
            if (tuple == null)
            {
                throw new ArgumentNullException("tuple");
            }

            this.subscriberInfos.Add(tuple.Item1, tuple);
            return this;
        }

        ////private static bool HasExpired(ISubscriberMetadata subscriberMetaData)
        ////{
        ////    var nextstart = subscriberMetaData.StartTime + subscriberMetaData.TimeToExpire;
        ////    if (DateTime.Compare(DateTime.Now, nextstart) > 0)
        ////    {
        ////        return true;
        ////    }

        ////    return false;
        ////}

        private static string GetSubscriptionId(string messageId, string subscriberName)
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append(":SubScriber::").Append(subscriberName).Append("::MessageID::").Append(messageId).Append(":").ToString();
        }

        private static MessagePacket<T> CreateSingleSubscriberMessagePacket(ISubscriber<T> subscriber, MessagePacket<T> messagePacket, bool failedOrTimedOut)
        {
            if (messagePacket == null)
            {
                throw new ArgumentNullException("messagePacket");
            }

            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }

            List<ISubscriberMetadata> metadatalist = new List<ISubscriberMetadata>();
            ISubscriberMetadata subscribermetadata = new SubscriberMetadata()
            {
                ////need to add abort count and think thru appropriate start time
                Name = subscriber.GetType().Name,
                TimeToExpire = subscriber.TimeToExpire,
                StartTime = subscriber.StartTime,
                RetryCount = subscriber.AbortCount,
                FailedOrTimedOutTime = subscriber.AbortedTime,
                FailedOrTimedOut = failedOrTimedOut
            };
            metadatalist.Add(subscribermetadata);
            var newMessagePacket = new MessagePacket<T>((T)messagePacket.Body, metadatalist);
            
            if ((messagePacket.MessageId != null) && (messagePacket.MessageId != 0))
            {
                newMessagePacket.MessageId = messagePacket.MessageId;
            }
            else
            {
                newMessagePacket.MessageId = Convert.ToInt32(subscriber.MessageId);
            }

            if (newMessagePacket.MessageId == null)
            {
                throw new ArgumentException("MessageId is null, and it can't be");
            }

            return newMessagePacket;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Error message needs to be swallowed, next processing run will try again")]
        private bool HandleSingleSubscriberforMessage(MessagePacket<T> messagePacket, string messageId)
        {
            var metaData = messagePacket.SubscriberMetadataList[0];
            string subscriptionId = GetSubscriptionId(messageId, metaData.Name);
            Trace.WriteLine("About to check if I can process: " + subscriptionId);

            ISubscriber<T> newSubscription = null;

            if (metaData.CanProcess())
            {
                var subscriberInfo = this.subscriberInfos.FirstOrDefault(si => si.Key == metaData.Name);
                newSubscription = (ISubscriber<T>)Activator.CreateInstance(subscriberInfo.Value.Item2);
                newSubscription.Name = subscriberInfo.Value.Item1;
                newSubscription.TimeToExpire = subscriberInfo.Value.Item3;
                newSubscription.Id = subscriptionId;
                newSubscription.AbortCount = metaData.RetryCount;
                if (metaData.FailedOrTimedOut == true)
                {
                    Debug.Assert(newSubscription.AbortCount > 0, "Abort count did not exceed 0 this is an invalid condition");
                }

                var cancellationTokenSource = new CancellationTokenSource(newSubscription.TimeToExpire);

                Task.Run(async () =>
                {
                    await newSubscription.RunAsync((T)messagePacket.Body, cancellationTokenSource.Token);
                    return true;
                })
                .ContinueWith(anticedant =>
                {
                    switch (anticedant.Status)
                    {
                        case TaskStatus.RanToCompletion:
#if DEBUG
                            Counter.Increment(16);
#endif
                            this.StorageProvider.UpdateMessageStore(CreateSingleSubscriberMessagePacket(newSubscription, messagePacket, false));
                            this.StorageProvider.SubscriberGroupCompletedForMessage(messageId);
                            break;
                        case TaskStatus.Faulted:
                            try
                            {
                                newSubscription.Abort();
                                this.StorageProvider.UpdateMessageStore(CreateSingleSubscriberMessagePacket(newSubscription, messagePacket, true));
#if DEBUG
                                Counter.Increment(20); 
#endif
                            }
                            catch
                            {
                                break;
                            }

                            this.StorageProvider.SubscriberGroupCompletedForMessage(messageId);
                            break;
                        case TaskStatus.Canceled:
                            try
                            {
                                newSubscription.Abort();
                                this.StorageProvider.UpdateMessageStore(CreateSingleSubscriberMessagePacket(newSubscription, messagePacket, true));
#if DEBUG
                                Counter.Increment(18); 
#endif
                            }
                            catch
                            {
                                break;
                            }

                            this.StorageProvider.SubscriberGroupCompletedForMessage(messageId);
                            break;
                    }

                    cancellationTokenSource.Dispose();
                });
            }

            return true;
        }

        ////private bool IsReady()
        ////{
        ////    if (this.storeageProvider == null)
        ////    {
        ////        return false;
        ////    }

        ////    if (this.subscriberInfos.Count == 0)
        ////    {
        ////        return false; // will not work with no subscribers
        ////    }

        ////    return true;
        ////}

        private void RunSubscriptions(MessagePacket<T> messagePacket, string messageId, SubscribersCollection<T> subscribersForThisMessage)
        {
            Task.Run(async () =>
            {
                foreach (var subscriber in subscribersForThisMessage)
                {
                    string newSubscriptionId = GetSubscriptionId(messageId, subscriber.Name);
                    subscriber.Id = newSubscriptionId;
                    subscriber.MessageId = messageId;
                    var cancellationToken = new CancellationTokenSource(subscriber.TimeToExpire).Token;

                    try
                    {
                        bool result = await subscriber.RunAsync((T)messagePacket.Body, cancellationToken);
                        this.StorageProvider.UpdateMessageStore(CreateSingleSubscriberMessagePacket(subscriber, messagePacket, false));
#if DEBUG
                        Counter.Increment(14); 
#endif
                    }
                    catch (OperationCanceledException)
                    {
                        subscriber.Abort();
                        this.StorageProvider.UpdateMessageStore(CreateSingleSubscriberMessagePacket(subscriber, messagePacket, true));
                        Counter.Increment(12);
                        ////Trace.WriteLine("This task timed out : " + subscriber.Name + " The timeout timespan was: " + subscriber.TimeToExpire.TotalMilliseconds + " ms");
                    }
                    catch (Exception)
                    {
                        subscriber.Abort();
                        this.StorageProvider.UpdateMessageStore(CreateSingleSubscriberMessagePacket(subscriber, messagePacket, true));
#if DEBUG
                        Counter.Increment(13); 
#endif
                        ////Trace.WriteLine("Request failed: " + newSubscriptionId + " newMessagId" + newMessagId + " " + anticedant.Exception.InnerException.ToString());    
                    }
                }
            }).ContinueWith((Anticedant) =>
            {
                this.StorageProvider.SubscriberGroupCompletedForMessage(messageId);
            });
        }
    }
}
