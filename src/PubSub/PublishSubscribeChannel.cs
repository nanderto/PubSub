using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Phantom.PubSub;
using System.Threading.Tasks;

namespace Phantom.PubSub
{
    public class PublishSubscribeChannel<T> : IPublishSubscribeChannel<T>, IDisposable
    {
        public PublishSubscribeChannel()
        {
        }

        // receiver will look at queue read messages and fire calls to subscribers
        // find all subscribers and call them in order
        // subsciber will be
        
        IQueueProvider<T> QueueProvider;
        
        System.Timers.Timer timer;

        public PublishSubscribeChannel(IQueueProvider<T> QueuePrivoder)
        {
            this.QueueProvider = QueuePrivoder;
            this.ActiveSubscriptions = new ActiveSubscriptions<T>();
            this.timer = new System.Timers.Timer(10000);
            this.timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        private static bool ProcessRunning = false;
        
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
           // this.Stop();
            try
            {
                ////todo need to look at throttling the calls here
                if (!ProcessRunning)
                {
                    ProcessRunning = true;
                    this.ProcessBatch();
                    this.CheckProcessingStatus();
                    ProcessRunning = false;
                }
               //// this.timer.Stop();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Error Policy");
            }
            finally
            {
            }
        }

        private void CheckProcessingStatus()
        {
            ////look for subscribers that have not completed by ther expirery time
            this.ActiveSubscriptions.ExpireOldSubscriptions();
        }

        internal static object QueueLock = new object();

        private bool RemoveFromQueue(string MessageId, MessageStatus<T> TrackStartedFinishedStatus)
        {
            bool result = false;
            // need to lock the removal from queue with the system wide collection that is tracking its status
            // if it reoves any subscription it will return true and attempt to remove from the queue
            lock (QueueLock)
            {
                result = this.QueueProvider.RemoveFromQueue(MessageId);
                var result2 = this.ActiveSubscriptions.RemoveIfExists(TrackStartedFinishedStatus);
            }

            return true;
        }
       
        void subscriber_OnProcessCompletedEvent(object sender, ProcessCompletededEventArgs e)
        {
            ProcessCompleted(e.MessageId, e.SubscriptionId, e.CurrentSubscription as ISubscriber<T>, e.SubScriptionStatus as IMessageStatus<T>, e.Subscriptions as MessageStatus<T>);
        }

        void subscriber_OnProcessStartedEvent(object sender, ProcessStartedEventArgs e)
        {
            ProcessStarted(e.MessageId, e.SubscriptionId, (ISubscriber<T>)e.CurrentSubscription, (IMessageStatus<T>)e.SubScriptionStatus, (List<IMessageStatus<T>>)e.Subscriptions);
        }

        /// <summary>
        /// start will start a timer
        /// this will start the batch processing.
        /// It sould not be called untill all set up is completed
        /// The timers job is to inspect if batches have been completed
        /// If not completed then check if the operations have timed out.
        /// If they have timed out then we should abort the operations and restrt after waiting period
        /// the waiting perior will exponentionaly increase
        /// If a message is not completed atthe end then it needs to go yo a dead message que
        /// </summary>
        public void Start()
        {
            if (IsReady(this))
            {
                this.timer.Start();
            }
            else
            {
                throw new Exception("Start was called without the component being ready to start - this is a code issue");
            }
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        private bool IsReady(PublishSubscribeChannel<T> messageReceiver)
        {
            if (this.QueueProvider == null) return false;
            //// if(this.MessageIds == null) return false;
            if (this.timer == null) return false;
            //// if (!GetSubScribersCalled) return false; called in timer
            return true;
        }

        /// <summary>
        /// Puts a message on the Queue, and passes the message to a new thread for processing.
        /// This component does read the message from the Que to start processing. It takes the same message that is added to the Queue and starts processing
        /// as soon as it is guaranteed to be on the queue.
        /// </summary>
        /// <param name="message"></param>
        public void PutMessage(T message)
        {
            string result = this.QueueProvider.PutMessage(message);
            Task.Factory.StartNew(() => HandleMessage(message, result));
        }

        /// <summary>
        /// Batch processing alternative, this is used as part of the clean up process for Messages that expire prior to being handled. 
        /// </summary>
        public void ProcessBatch()
        {
            LogEntry log = new LogEntry();
            log.Message = ("ProcessBatch Should fire every 10 seconds:" + DateTime.Now.ToString());
            Logger.Write(log);
            this.QueueProvider.ProcessQueueAsBatch(HandleMessage);
        }

        //private bool GetSubScribersCalled = false;
        //// public static DateTime FinishedTime;
        
        /// <summary>
        /// This method receives a single message and processes all of the subscribers for each individual message. The subscribers are processed in parallel
        /// to ensure that one long running process will not impact the others.
        /// When all subscribers have indicate they have completed a flag is set to indicate that this message has completed publishing, and it is removed 
        /// from the Queue
        /// This method uses the Parallel.ForEach<T> method, so Visual Studio parallel debugging can be used.
        /// </summary>
        /// <param name="message">The message of type T</param>
        /// <param name="MessageId">The ID of the message</param>
        public void HandleMessage(T message, string MessageId)
        {
            ////LogEntry log = new LogEntry();
            ////log.Message = ("HandleMessage: MessageId: " + MessageId);
            ////Logger.Write(log);

            // create a new instance and use in this method
            // each new call should get a new instance.
            // this is a list of the subscribers for the message being handled in this thread 
            MessageStatus<T> trackStartedFinishedStatus = new MessageStatus<T>();
            var MessageStatusTrackers = GetMessageStatusTrackers();
            foreach (var item in MessageStatusTrackers)
            {
                trackStartedFinishedStatus.Add(item);
            }

            Parallel.ForEach<IMessageStatus<T>>(trackStartedFinishedStatus, (IMessageStatus<T> SubScriptionStatus) =>
            {
                ////LogEntry log2 = new LogEntry();
                ////log2.Message = ("Parallel.loop: subscriber.Name: " + SubScriptionStatus.Name +  ":: MessageID: " + MessageId + "::");
                ////Logger.Write(log2);

                // add flag that tracks if this particular subscriber for this particular message has been started
                // var current = trackStartedFinishedStatus.Find(s => s.Name == subscriber.Name);
                // current.StartedProcessing = true;
                // SubScriptionStatus.StartedProcessing = true;

                // check if this message can be processed - it may already be in process after being launched with 
                // a earlier batch
                string newSubscriptionId = " SubScriber: " + SubScriptionStatus.Name + ":: MessageID: " + MessageId + "::";
                SubScriptionStatus.Id = newSubscriptionId;
                //SubScriptionStatus.MessageId = MessageId;

                if (!this.ActiveSubscriptions.Reprocess(newSubscriptionId, message, MessageId, SubScriptionStatus, trackStartedFinishedStatus))
                {
                    // get a subscription for this subscriber - basically an instance of the subscriber that 
                    // will run and track this particuklar message and for this particular subscriber.
                    // This is probably not a very good design because
                    // i am using the same object for 2 different purposes (thats gotta at least break the SRP)
                    // but i started thinking I would handle them in baches and get new ones each time
                    // and now i realise that will not work. probaly need to change the name as well
                    // subscriber & subscription subscriber is the process that is subscribing to the event
                    // and the subscription is the message being processed
                    ISubscriber<T> newSubscription = GetSubscription(SubScriptionStatus);
                    newSubscription.MessageStatusTracker = SubScriptionStatus;
                    
                    // wire up the events
                    newSubscription.OnProcessStartedEvent += new OnProcessStartedEvent(subscriber_OnProcessStartedEvent);
                    newSubscription.OnProcessCompletedEvent += new OnProcessCompletedEvent(subscriber_OnProcessCompletedEvent);
                    
                    // newSubscription.OnSubscriptionStatusUpdatedEvent += new OnSubscriptionStatusUpdatedEvent(SubScriptionStatus.newSubscription_OnSubscriptionStatusUpdatedEvent);
                    // SubScriptionStatus.OnSubscriptionStatusUpdatedEvent += new OnSubscriptionStatusUpdatedEvent(SubScriptionStatus_OnSubscriptionStatusUpdatedEvent);
                    // create an ID for this specific messanger and subscriber
                    newSubscription.Id = newSubscriptionId;
                    newSubscription.MessageId = MessageId;
                    
                    // initiate process
                    ////newSubscription.StartedProcessing = true;
                    ////this.ActiveSubscriptions.Add(activeSubscription);
                    this.ActiveSubscriptions.AddActiveSubscription(newSubscription);
                    
                    // hmm should probabbly be done asynchronosly although this is the last line of code in a parallel process any way,
                    newSubscription.Run(message, MessageId, newSubscription.Id, SubScriptionStatus, trackStartedFinishedStatus);
                }
            });
        }

        public ISubscriber<T> GetSubscription(IMessageStatus<T> subscriber)
        {
           System.Type outValue = null;
           if(MessageStatus.TryGetValue(subscriber.Name, out outValue))
           {
                ISubscriber<T> retValue = ( ISubscriber<T>) Activator.CreateInstance(outValue);
               Type constructedType = Type.GetType(outValue.Name);
               return retValue;
           }
           else
           {
               throw new ArgumentOutOfRangeException("Cannot find the subscriber expected. Name: " + subscriber.Name);
           }
        }

        /// <summary>
        /// This method is provide for the inheriting member to override and allow some pre-processing 
        /// </summary>
        /// <param name="MessageID"></param>
        /// <param name="SubscriberId"></param>
        /// <param name="CurrentSubscription"></param>
        /// <param name="SubscriptionStatus"></param>
        /// <param name="TrackStartedFinishedStatus"></param>
        public virtual void ProcessStarted(string MessageID, string SubscriberId, ISubscriber<T> CurrentSubscription, IMessageStatus<T> SubscriptionStatus, List<IMessageStatus<T>> TrackStartedFinishedStatus)
        {

        }

        /// <summary>
        /// Handles all post processing. Sets the status of the Subscriber to complete. Sets the status of the subsciption to complete.
        /// Checks if all of the other subscribers for this specific message have both started and completed, if they have then the function will
        /// call the queue provider to remove the message from the queue.
        /// </summary>
        /// <param name="MessageID">ID for the message that is being dealt with, generated by Queue provider. Will be the same for this message each
        /// time it is read from the queue. </param>
        /// <param name="SubscriberId">ID for this message and subscription. Will be the same for this message / subscriber combination every time
        /// the Queue provider processes this message.</param>
        /// <param name="CurrentSubscriber">The subscriber that was processing this message and raised the process completed event passing its self into the event args.
        /// This parameter is held in a collection of subscribers for this object</param>
        /// <param name="SubccriptionStatus">Tracks the status of this particular subscriber process. (combination of message and subscriber) Is held in the 
        /// SubscriberStatus<T> collection parameter. It is passed in so that it is not necessary to find in the tracking collection to set its status</param>
        /// <param name="TrackStartedFinishedStatus">Collection of subscribers for this specific message, these subscribers are used to track the status of the 
        /// subscription process for each message and subscriber combination</param>
        public void ProcessCompleted(string MessageID, string SubscriberId, ISubscriber<T> CurrentSubscriber, IMessageStatus<T> SubccriptionStatus, MessageStatus<T> TrackStartedFinishedStatus)
        {
            ////CurrentSubscriber.FinishedProcessing = true;
            ////SubccriptionStatus.FinishedProcessing = true;

            ////LogEntry log = new LogEntry();
            ////log.Message = ("FinishedProcessing: " + SubscriberId);
            ////Logger.Write(log); 

            // Check if all subscribers have both started and completed. If they have tell queue provider to remove from the queue as it has been successfully processed
            if (TrackStartedFinishedStatus.IfAllSubscribersStartedandCompletedLockandRemove(RemoveFromQueue, MessageID))
            {
            }
        }

        public ActiveSubscriptions<T> ActiveSubscriptions { get; set; }

        #region IMessageReceiver<T> Members

        public Dictionary<string, Type> MessageStatus = new Dictionary<string, Type>();
        
        public MessageStatus<T> GetMessageStatusTrackers()
        {
            var messageStatus = new MessageStatus<T>();
            foreach (var item in MessageStatus)
            {
                var mst = new MessageStatusTracker<T>() as IMessageStatus<T>;
                mst.Name = item.Value.Name;
                messageStatus.Add(mst);
            }
            return messageStatus;
        }

        //public virtual MessageStatus<T> GetSubScriberStatuses(MessageStatus<T> Subscribers)
        //{
        //   // this.GetSubScribersCalled = true;

        //    return Subscribers;
        //}

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PublishSubscribeChannel() 
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.timer.Dispose();
            }
        }

        #endregion

        public IPublishSubscribeChannel<T> AddSubscriber(Type type)
        {
            MessageStatus.Add(type.Name, type);
            return this;
        }



    }
}
