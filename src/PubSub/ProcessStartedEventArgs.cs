namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProcessStartedEventArgs : EventArgs
    {
        private string name;

        public ProcessStartedEventArgs(string name)
        {
            this.name = name;
        }

        public ProcessStartedEventArgs(string messageId, string subscriptionId, object currentSubscription, object subScriptionStatus, object subscriptions)
        {
            this.MessageId = messageId;
            this.SubscriptionId = subscriptionId;
            this.Subscriptions = subscriptions;
            this.CurrentSubscription = currentSubscription;
            this.SubScriptionStatus = subScriptionStatus;
        }
        
        public string MessageId { get; set; }
        
        public string SubscriptionId { get; set; }
        
        public object Subscriptions { get; set; }
        
        public object CurrentSubscription { get; set; }
        
        public object SubScriptionStatus { get; set; }
    }
}
