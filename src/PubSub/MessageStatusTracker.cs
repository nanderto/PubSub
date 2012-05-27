using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantom.PubSub
{
    public class MessageStatusTracker<T> : IMessageStatus<T>
    {
        public bool FinishedProcessing { get; set; }

        public bool StartedProcessing { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
