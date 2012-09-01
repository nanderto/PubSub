namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ISubscriberInfo<T>
    {
        IPublishSubscribeChannel<T> WithTimeToExpire(TimeSpan timeSpan);
    }

    public class SubscriberInfo<T> : ISubscriberInfo<T>
    {
        private Type subscriberType = null;

        private IPublishSubscribeChannel<T> publishSubscribeChannel = null;

        public SubscriberInfo(Type subscriberType, IPublishSubscribeChannel<T> publishSubscribeChannel)
        {
            this.subscriberType = subscriberType;
            this.publishSubscribeChannel = publishSubscribeChannel;
        }

        public IPublishSubscribeChannel<T> WithTimeToExpire(TimeSpan timeSpan)
        {
            return ((PublishSubscribeChannel<T>) this.publishSubscribeChannel).AddSubscriberInfo(Tuple.Create(this.subscriberType.Name, this.subscriberType, timeSpan));
        }
    }
}
