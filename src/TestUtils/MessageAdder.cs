using Phantom.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TestUtils
{
    public class MessageAdder<T>
    {
        public MessagePacket<T> messagePacket;
        private string QueueName;
        private T message;
        public List<ISubscriberMetadata> metadatalist;

        public MessageAdder(T message, string queueName)
        {
            this.message = message;
            this.QueueName = queueName;
        }

        public string AddMessage()
        {

            Message recoverableMessage = new Message();
            recoverableMessage.Body = messagePacket;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\" + QueueName);
            try
            {
                msgQ.Send(recoverableMessage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Exception::::::: " + ex.ToString());
            }
            finally
            {
            }
            return recoverableMessage.Id;
        }

        public MessageAdder<T> WithSubscriberMetadataFor(Type SubscriberType, TimeSpan timeToExpire)
        {
            if (metadatalist == null)
            {
                metadatalist = new List<ISubscriberMetadata>();
            }
            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = SubscriberType.Name,// TestHelper.CleanupName(SubscriberType.ToString()),
                TimeToExpire = timeToExpire,
                StartTime = DateTime.Now
            };

            metadatalist.Add(subscribermetadata1);

            if (messagePacket == null)
            {
                messagePacket = new MessagePacket<T>(message, metadatalist);
            }
            return this;
        }


    }
}
