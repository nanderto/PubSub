namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProcessCompletedEventArgs : EventArgs
    {
        public ProcessCompletedEventArgs()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCompletedEventArgs" /> class.
        /// This Constructor for ProcessCompletededEventArgs is used to set properties. This Event
        /// arg is used when the Queue provider is signalling that it has completed processing
        /// </summary>
        /// <param name="currentSubscription">Is the current Subscription, which containes all required information</param>
        public ProcessCompletedEventArgs(object currentSubscription)
        {
            this.CurrentSubscription = currentSubscription;
        }

        public object CurrentSubscription { get; set; }
    }
}
