using Phantom.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtils
{
    public class MessageBuilder<T>
    {
        public MessagePacket<T> messagePacket;
        private string QueueName;
        private T message;
        public List<ISubscriberMetadata> metadatalist;

        public MessageBuilder(T message, string queueName)
        {
            this.message = message;
            this.QueueName = queueName;
        }

        public MessageBuilder<T> WithSubscriberMetadataFor(Type SubscriberType, bool failedOrTimedOut, bool completed)
        {
            if (metadatalist == null)
            {
                metadatalist = new List<ISubscriberMetadata>();
            }

            var r = new Random();
            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = SubscriberType.Name,// TestHelper.CleanupName(SubscriberType.ToString()),
                FailedOrTimedOut = failedOrTimedOut,
                Completed = completed,
                //TimeToExpire = timeToExpire,
                StartTime = DateTime.Now,
                Id = r.Next().ToString()
            };

            metadatalist.Add(subscribermetadata1);

            if (messagePacket == null)
            {
                messagePacket = new MessagePacket<T>(message, metadatalist);
            }
            return this;
        }

        public MessagePacket<T> GetMessage()
        {
            Random r = new Random();

            messagePacket.MessageId = r.Next();
            return messagePacket;
        }

        public List<ISubscriberMetadata> GetMetadataList()
        {
            return metadatalist;
        }
    }
}
