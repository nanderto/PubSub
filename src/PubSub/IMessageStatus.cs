using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phantom.PubSub;

namespace Phantom.PubSub
{
    public interface IMessageStatus<T>
    {
        bool FinishedProcessing { get; set; }
        
        bool StartedProcessing { get; set; }
        
        string Id { get; set; }
        
        string Name { get; set; }
        
        //string MessageId { get; set; }
        
        //TimeSpan TimeToExpire { get; set; }
    }
}
