namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ISubscriberMetadata
    {
        string Id { get; set; }

        string Name { get; set; }

        DateTime StartTime { get; set; }

        TimeSpan TimeToExpire { get; set; }

        bool CanProcess();

    }
}
