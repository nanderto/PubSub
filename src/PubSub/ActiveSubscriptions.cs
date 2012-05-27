namespace Phantom.PubSub
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Phantom.PubSub;

    /// <summary>
    /// Collection that keeps each of the subscriptions for each message subscriber combination.
    /// There will be only one collection for each Type of Publish Subscribe Channel
    /// Used to check if a process has gon past its expirery time.
    /// </summary>
    /// <typeparam name="T">Type of Message</typeparam>
    public class ActiveSubscriptions<T> : ConcurrentDictionary<string, ISubscriber<T>>
    {
        /// <summary>
        /// Reprocess will start the process again. It checks if the message is already being processed
        /// or if not it checks if is is time to start reprocessing 
        /// </summary>
        /// <param name="subscriptionId">Compond key combining messageID and subsciption name, Each subscription must have a unique name</param>
        /// <param name="message">the object/meddage to process thru the subscriber</param>
        /// <param name="messageId">Identifies this specific message</param>
        /// <param name="subscriptionStatus">collection of subscribers local to the calling method that tracks
        /// if each subscription has been started for this specific message</param>
        /// <param name="messageStatusTrackIfStartedsndFinished"></param>
        /// <returns>True if this method handles reprocessing. Will return false even if it did not initiate reprocessing
        /// because it handled it. Returns true if the active subscription does not exist in the list
        /// </returns>
        public bool Reprocess(string subscriptionId, T message, string messageId, IMessageStatus<T> subscriptionStatus, List<IMessageStatus<T>> messageStatusTrackIfStartedsndFinished)
        {
            ////LogEntry log2 = new LogEntry();
            ////log2.Message = ("Reprocess:SubscriptionId " + SubscriptionId + "MessageId: " + MessageId);
            ////Logger.Write(log2);

            ISubscriber<T> existingSubscription = null;
            bool matchExists = this.TryGetValue(subscriptionId, out existingSubscription);

            if (matchExists)
            {
                ////LogEntry log = new LogEntry();
                ////log.Message = ("Reprocessing step1 has been processed before: MessageId: " + MessageId);
                ////Logger.Write(log);

                // ISubscriber<T> existingSubscription = existingSubscriptions.First();
                if (existingSubscription.CanProcess())
                {
                    ////log.Message = log.Message + (" Reprocessing: yes: MessageId: " + MessageId);
                    ////Logger.Write(log);
                    // wire up the statustracking so it can be notified when a status is on the active subscription is updated 
                    // existingSubscription.OnSubscriptionStatusUpdatedEvent += new OnSubscriptionStatusUpdatedEvent(SubscriptionStatus.newSubscription_OnSubscriptionStatusUpdatedEvent);
                    existingSubscription.Run(message, messageId, existingSubscription.Id, subscriptionStatus, messageStatusTrackIfStartedsndFinished);
                    return true;
                }
            }

            return false;
        }

        public ISubscriber<T> AddActiveSubscription(ISubscriber<T> ActiveSubscription)
        {
            this.TryAdd(ActiveSubscription.Id, ActiveSubscription);
            return ActiveSubscription;
        }

        public void ExpireOldSubscriptions()
        {
            ////LogEntry log = new LogEntry();
            ////log.Message = ("ExpireOldSubscriptions::Total number of active Subscriptions: " + this.Count());
            ////Logger.Write(log);

            ISubscriber<T> subscriber;
            foreach (var item in this)
            {
                subscriber = item.Value;
                if (subscriber.Aborted == false)
                {
                    var now = DateTime.Now;
                    if (DateTime.Compare(subscriber.ExpireTime, now) < 0)
                    {
                        ////log.Message = ("Aborting Subscription::MessageID: " + subscriber.MessageId);
                        ////Logger.Write(log);
                        subscriber.Abort();
                    }
                }
            }
        }

        public bool RemoveIfExists(List<IMessageStatus<T>> SubscriptionStatus)
        {
            bool result = false;
            bool thisResult = false;
            //LogEntry log = new LogEntry();
            //log.Message = ("IN RemoveIfExists: ");
            //Logger.Write(log);

            foreach (var item in SubscriptionStatus)
            {
                //log.Message = ("IN RemoveIfExists::Trying to remove SubscriptionID: " + item.Id);
                //Logger.Write(log);

                ISubscriber<T> RemovedItem;
                thisResult = false;
                thisResult = this.TryRemove(item.Id, out RemovedItem);
                if (thisResult) result = true; // Return true if any are found
            }

            return result;
        }
    }
}
