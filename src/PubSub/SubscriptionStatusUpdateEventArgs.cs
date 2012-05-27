namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SubscriptionStatusUpdateEventArgs : EventArgs
    {
        public SubscriptionStatusUpdateEventArgs(SubscriptionStatusChange statusChange, bool subscriptionStarted, bool subscriptionCompleted, bool aborted, DateTime timeOccurred)
        {
            if (statusChange == SubscriptionStatusChange.Aborted) { this.Aborted = aborted; } 
            if (statusChange == SubscriptionStatusChange.Finished) this.SubscriptionCompleted = subscriptionCompleted;
            if (statusChange == SubscriptionStatusChange.Started) this.SubscriptionStarted = subscriptionStarted;
            this.StatusChange = statusChange;
            this.TimeOccured = timeOccurred;
        }

        public bool SubscriptionStarted { get; set; }
        
        public bool SubscriptionCompleted { get; set; }
        
        public bool Aborted { get; set; }
        
        public DateTime TimeOccured { get; set; }
        
        public SubscriptionStatusChange StatusChange { get; set; }
    }

    /// <summary>
    /// Used to show status change of associated Subscription
    /// </summary>
    public enum SubscriptionStatusChange
    {   
        /// <summary>
        /// Subscription has stated processing
        /// </summary>
        Started,
        
        /// <summary>
        /// Subscription has finished processing
        /// </summary>
        Finished,
        
        /// <summary>
        /// Subscription processing has been aborted because it passed the expiration time
        /// </summary>
        Aborted
    }
}
