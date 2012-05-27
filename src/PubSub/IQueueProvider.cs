using System;
using System.Collections.Generic;

namespace Phantom.PubSub
{
    public delegate void MessageHandlingCompleted(string MessageID, string SubscriberID);
    public delegate void MessageHandlingStarted(string MessageID, string SubscriberID);
    public delegate bool RemoveMessageFromQueue<T>(string MessageId, MessageStatus<T> StartFinishStatus);

    public interface IQueueProvider<T>
    {
        string ConfigureQueue(string queueName);
         
        string Name { get; set; }
        
        void OnMessageSentEventHandler(MessageSentEventArgs e);
        
        T ReadQueue(out string correlationId);
        
        string PutMessage(T message);
        
        void SetUpWatchQueue(IQueueProvider<T> queueProvider);
        
        List<ISubscriber<T>> Subscribers { get; set; }

        bool RemoveFromQueue(string correlationId);
        
        void ProcessQueueAsBatch(MsmqQueueProvider<T>.MessageHandlingInitiated messageHandlingInitiated);
        
        bool IsQueueEmpty();
    }

}
