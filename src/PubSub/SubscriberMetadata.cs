namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class SubscriberMetadata : ISubscriberMetadata
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan TimeToExpire { get; set; }
    }
}
