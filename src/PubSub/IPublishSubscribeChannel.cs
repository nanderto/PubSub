namespace Phantom.PubSub
{
using System;
    using System.Collections.Generic;

    public interface IPublishSubscribeChannel<T>
    {
        Subscribers<T> GetSubscriptions();
        
        void ProcessBatch();
        
        void PublishMessage(T message);

        ISubscriberInfo<T> AddSubscriberType(Type type);

        IPublishSubscribeChannel<T> AddSubscriberInfo(Tuple<string, Type, TimeSpan> tuple);

    }
}
