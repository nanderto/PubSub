using System;

namespace Phantom.PubSub
{
    public interface IPublishSubscribeChannel<T>
    {
        MessageStatus<T> GetMessageStatusTrackers();

        ISubscriber<T> GetSubscription(IMessageStatus<T> subscriber);
        
        void ProcessBatch();
        
        void Stop();
       
        void Start();
       
        void PutMessage(T message);

        IPublishSubscribeChannel<T> AddSubscriber(Type type);
    }
}
