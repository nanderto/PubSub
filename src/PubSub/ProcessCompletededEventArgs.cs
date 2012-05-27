using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantom.PubSub
{
    public class ProcessCompletededEventArgs : EventArgs
    {
        private string Name;

        public ProcessCompletededEventArgs(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// This Constructor for ProcessCompletededEventArgs is used to set properties. This Event
        /// arg is used when the Que provider is signalling that it has completed processing
        /// </summary>
        /// <param name="messageId">Unique ID for message (gennerally assigned by Queue Provider)</param>
        /// <param name="subscriptionId"></param>
        /// <param name="currentSubscription"></param>
        /// <param name="subScriptionStatus"></param>
        /// <param name="subscriptions"></param>
        public ProcessCompletededEventArgs(string messageId, string subscriptionId, object currentSubscription, object subScriptionStatus, object subscriptions)
        {
            this.MessageId = messageId;
            this.SubscriptionId = subscriptionId;
            this.Subscriptions = subscriptions;
            this.CurrentSubscription = currentSubscription;
            this.SubScriptionStatus = subScriptionStatus;
        }

        /// <summary>
        /// Gets or sets the Unique ID for message (generally assigned by Queue Provider)
        /// </summary>
        public string MessageId { get; set; }
         
        public string SubscriptionId { get; set; }
        
        public object Subscriptions { get; set; }

        public object CurrentSubscription { get; set; }
        
        public object SubScriptionStatus { get; set; }
    }
}
