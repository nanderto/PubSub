using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;


namespace Phantom.PubSub
{
    public class PublishMessageFilter<T> : Filterbase<T>
    {
        //private IQueueProvider<T> Queue;
        private IPublishSubscribeChannel<T> PublishSubscribeChannel;

        public PublishMessageFilter(IPublishSubscribeChannel<T> PublishSubscribeChannel)
        {
            this.PublishSubscribeChannel = PublishSubscribeChannel;
        }

        //I dont think this is needed
        //public PublishMessageFilter(IPublishSubscribeChannel<T> PublishSubscribeChannel, IQueueProvider<T> Queue)
        //{
        //    this.PublishSubscribeChannel = PublishSubscribeChannel;
        //    this.Queue = Queue;
        //}  
        
        protected override T Process(T input)
        {
           // this.Queue.PutMessage(input);
            this.PublishSubscribeChannel.PutMessage(input);
            return input;

        }
    }
}
