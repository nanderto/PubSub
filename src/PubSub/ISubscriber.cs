namespace Phantom.PubSub
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Phantom.PubSub;

    public interface ISubscriber<T>
    {
        event OnProcessStartedEventHandler OnProcessStartedEventHandler;        
        
        event OnProcessCompletedEventHandler OnProcessCompletedEventHandler;

        int AbortCount { get; set; }

        string Name { get; set; }
        
        string Id { get; set; }
       
        string MessageId { get; set; }

        Subscribers<T> SubscribersForThisMessage { get; set; }
        
        bool Aborted { get; set; }

        TimeSpan TimeToExpire { get; set; }

        TimeSpan DefaultTimeToExpire { get; }
        
        DateTime ExpireTime { get; set; }
        
        DateTime StartTime { get; set; }
        
        DateTime AbortedTime { get; set; }

        bool FinishedProcessing { get; set; }

        bool StartedProcessing { get; set; }

        bool Process(T input);

        bool Abort();

        bool CanProcess();

        bool Run(T message);

        Task<bool> RunAsync(T message, CancellationToken cancellationToken);
    }
}