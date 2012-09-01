namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ProcessStartedEventArgs : EventArgs
    {
        public ProcessStartedEventArgs()
        {
        }

        public ProcessStartedEventArgs(object currentSubscription)
        {
            this.CurrentSubscription = currentSubscription;
        }
              
        public object CurrentSubscription { get; set; }
    }
}
