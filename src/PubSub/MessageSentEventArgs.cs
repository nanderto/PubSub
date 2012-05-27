using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantom.PubSub
{
    public class MessageSentEventArgs: EventArgs
    {
        private string Name;

        public MessageSentEventArgs(string Name)
        {
            this.Name = Name;
        }
        
        public string QueueName { get; set; }
        
        public string MessageType { get; set; }
    }
}
