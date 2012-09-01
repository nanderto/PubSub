namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Messaging;
    using System.Text;

    public class PublishMessageFilter<T> : FilterBase<T>
    {
        private IPublishSubscribeChannel<T> publishSubscribeChannel;

        public PublishMessageFilter(IPublishSubscribeChannel<T> publishSubscribeChannel)
        {
            this.publishSubscribeChannel = publishSubscribeChannel;
        }
        
        protected override T Process(T input)
        {
            this.publishSubscribeChannel.PublishMessage(input);
            return input;
        }
    }
}
