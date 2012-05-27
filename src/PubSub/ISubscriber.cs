namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Phantom.PubSub;

    public delegate void OnProcessStartedEvent(object sender, ProcessStartedEventArgs e);
    
    public delegate void OnProcessCompletedEvent(object sender, ProcessCompletededEventArgs e);

    public interface ISubscriber<T>
    {
        #region Part of Interface responsible for runing specific message subscription combination 
        
        event OnProcessStartedEvent OnProcessStartedEvent;
        
        event OnProcessCompletedEvent OnProcessCompletedEvent;

        string Name { get; set; }
        
        string Id { get; set; }
       
        string MessageId { get; set; }

        #endregion

        bool Process(T input);
        
        IMessageStatus<T> MessageStatusTracker { get; set; }   
        
        bool Aborted { get; set; }
        
        bool Abort();

        TimeSpan TimeToExpire { get; set; }
        
        DateTime ExpireTime { get; set; }
        
        DateTime StartTime { get; set; }
        
        DateTime AbortedTime { get; set; }

        bool Process(T message, string mMessageId, string subscriptionId, IMessageStatus<T> subScriptionStatus, List<IMessageStatus<T>> trackIfStarted);
       
        bool Run(T message, string messageId, string subscriptionId, IMessageStatus<T> subScriptionStatus, List<IMessageStatus<T>> trackIfStarted);
        
        bool CanProcess();
    }
}