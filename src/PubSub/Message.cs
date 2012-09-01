namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class MessagePacket<T> 
    {
        private T body = default(T);

        public MessagePacket(T message)
        {
            this.Body = message;
        }

        public MessagePacket(T message, List<ISubscriberMetadata> subscriberMetadataList)
        {
            this.Body = message;
            this.SubscriberMetadataList = subscriberMetadataList;
        } 

        public string Id { get; set; }

        public string Name { get; set; }

        public List<ISubscriberMetadata> SubscriberMetadataList { get; set; }

        public T Body
        {
            get { return this.body; }
            set { this.body = value; }
        }
    }
}
