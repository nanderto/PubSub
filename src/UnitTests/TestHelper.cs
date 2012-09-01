using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using Models = Entities;
using Phantom.PubSub;
using BusinessLogic;

namespace UnitTests
{
    public static class TestHelper
    {
        private static MessageQueue msgQ;
        public static void SetUpCleanTestQueue()
        {
            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");
            MessageQueue.Create(@".\private$\EntitiesUser");

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUserPoisonMessages"))
                MessageQueue.Delete(@".\private$\EntitiesUserPoisonMessages");
            MessageQueue.Create(@".\private$\EntitiesUserPoisonMessages");
        }

        public static void SetUpCleanTestQueue(string QueueName)
        {
            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\" + QueueName))
                MessageQueue.Delete(@".\private$\" + QueueName);
            MessageQueue.Create(@".\private$\" + QueueName);

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\" + QueueName + "PoisonMessages"))
                MessageQueue.Delete(@".\private$\" + QueueName + "PoisonMessages");
            MessageQueue.Create(@".\private$\" + QueueName + "PoisonMessages");
        }

        public static MessageQueue GetQueue()
        {
            msgQ = new MessageQueue(@".\private$\EntitiesUser");
            return msgQ;
        }

        public static bool IsQueueEmpty(string QueueName)
        {
            if (msgQ == null)
            {
                msgQ = new MessageQueue(@".\private$\" + QueueName);
            }

            bool isQueueEmpty = false;
            try
            {
                msgQ.Peek(new TimeSpan(0));
                //}
                isQueueEmpty = false;
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    isQueueEmpty = true;
                }
            }
            return isQueueEmpty;
        } 

        public static bool IsQueueEmpty()
        {
            if (msgQ == null)
            {
                msgQ = new MessageQueue(@".\private$\EntitiesUser");
            }

            bool isQueueEmpty = false;
            try
            {
               msgQ.Peek(new TimeSpan(0));
                //}
                isQueueEmpty = false;
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    isQueueEmpty = true;
                }
            }
            return isQueueEmpty;
        } 
        

        public static string AddABadMessage()
        {
            Models.User u = new Models.User();
            u.FirstName = "Gwen";
            u.LastName = "XXXX";

            Message recoverableMessage = new Message();
            recoverableMessage.Body = u;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
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

        private static MessagePacket<Models.User> GetMessagePacket(Models.User u)
        {
            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = "John",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            var subscribermetadata2 = new SubscriberMetadata()
            {
                Name = "Joe",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            List<ISubscriberMetadata> metadatalist = new List<ISubscriberMetadata>();
            metadatalist.Add(subscribermetadata1);
            metadatalist.Add(subscribermetadata2);

            var message = new MessagePacket<Models.User>(u, metadatalist);
            return message;
        }

        private static MessagePacket<Models.Message> GetMessagePacket(Models.Message u)
        {
            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = "John",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            var subscribermetadata2 = new SubscriberMetadata()
            {
                Name = "Joe",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            List<ISubscriberMetadata> metadatalist = new List<ISubscriberMetadata>();
            metadatalist.Add(subscribermetadata1);
            metadatalist.Add(subscribermetadata2);

            var message = new MessagePacket<Models.Message>(u, metadatalist);
            return message;
        }

        private static MessagePacket<Models.Message> GetMessagePacketwith1MillisecondTTE(Models.Message u)
        {
            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = "TestSubscriber`1",
                TimeToExpire = new TimeSpan(0, 0, 0, 0, 1),
                StartTime = DateTime.Now
            };

            var subscribermetadata2 = new SubscriberMetadata()
            {
                Name = "TestSubscriber2`1",
                TimeToExpire = new TimeSpan(0, 0, 0, 0, 1),
                StartTime = DateTime.Now
            };

            List<ISubscriberMetadata> metadatalist = new List<ISubscriberMetadata>();
            metadatalist.Add(subscribermetadata1);
            metadatalist.Add(subscribermetadata2);

            var message = new MessagePacket<Models.Message>(u, metadatalist);
            return message;
        }

        public static string AddAMessage(string QueueName)
        {
            Entities.User m = new Entities.User();
            m.FirstName = "Gwen";

            var message = GetMessagePacket(m);

            Message recoverableMessage = new Message();
            recoverableMessage.Body = message;
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
            return recoverableMessage.ToString();
        }

        public static Subscribers<T> GetSubscribers<T>()
        {
            var subscribers = new Subscribers<T>();
            ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
            sub.Name = "jackie";
            sub.Id = "SubscriptionID::jackie::1";
            sub.TimeToExpire = new TimeSpan(0,0,2);
            ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
            sub2.Name = "Rube";
            sub2.Id = "SubscriptionID::Rube::2";
            sub2.TimeToExpire = new TimeSpan(0, 0, 20);
            ISubscriber<T> sub3 = new TestSubscriber<T>() as ISubscriber<T>;
            sub3.Name = "Sid";
            sub3.Id = "SubscriptionID::Sid::3";
            sub3.TimeToExpire = new TimeSpan(0, 1, 20);

            subscribers.Add(sub);
            subscribers.Add(sub2);
            subscribers.Add(sub3);
            return subscribers;
        }

        public static ActiveSubscriptionsDictionary<T> AddSubscribers<T> (ActiveSubscriptionsDictionary<T> input, List<ISubscriber<T>> CollecctionToAdd)
        {
            foreach (var item in CollecctionToAdd)
            {
                input.AddActiveSubscription(item);
            }
            return input;
        }

        public static ActiveSubscriptionsDictionary<T> GetSpecialSubscribers<T>()
        {
            var subscribers = new ActiveSubscriptionsDictionary<T>();
            ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
            sub.Name = "XXX";
            sub.TimeToExpire = new TimeSpan(10000);
            sub.Id = "Subdcription ID::XXX";
            ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
            sub2.Name = "YYY";
            sub2.TimeToExpire = new TimeSpan(10000);
            sub2.Id = "Subdcription ID::YYY";
            ISubscriber<T> sub3 = new TestSubscriber<T>() as ISubscriber<T>;
            sub3.Name = "ZZZ";
            sub3.TimeToExpire = new TimeSpan(10000);
            sub3.Id = "Subdcription ID::ZZZ";
            subscribers.AddActiveSubscription(sub);
            subscribers.AddActiveSubscription(sub2);
            subscribers.AddActiveSubscription(sub3);
            return subscribers;
        }


        public static ActiveSubscriptionsDictionary<T> AddAllotofSubscriptions<T>(ActiveSubscriptionsDictionary<T> input, int NumberToAdd)
        {
            //AddTest a bunch of active subscriptions
            int i = 0;
            for (int j = 0; j < NumberToAdd; j++)
            {

                foreach (var item in TestHelper.GetSubscribers<T>())
                {
                    item.Id = " SubScription: " + item.Name + j + ":: MessageID: " + i + "000::";
                   // item.MessageStatusTracker = new MessageStatusTracker();
                    input.AddActiveSubscription(item);
                    i++;
                }
            }
            return input;
        }


        public static string AddAMessageMessageWith1MillisecondTTE()
        {
            Models.Message m = new Models.Message();
            m.Name = "Gwen";
            m.BatchNumber = 1;
            m.Guid = System.Guid.NewGuid();
            m.MessageID = "MessageID";
            m.SubscriptionID = "SubscriptionID";
            m.MessagePutTime = DateTime.Now;
            m.ID = 999;

            var message = GetMessagePacketwith1MillisecondTTE(m);

            Message recoverableMessage = new Message();
            recoverableMessage.Body = message;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesMessage");
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


        public static string AddAMessageMessage()
        {
            Models.Message m = new Models.Message();
            m.Name = "Gwen";
            m.BatchNumber = 1;
            m.Guid = System.Guid.NewGuid();
            m.MessageID = "MessageID";
            m.SubscriptionID = "SubscriptionID";
            m.MessagePutTime = DateTime.Now;

            var message = GetMessagePacketwith1MillisecondTTE(m);

            Message recoverableMessage = new Message();
            recoverableMessage.Body = message;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesMessage");
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

        public static IPublishSubscribeChannel<Models.Message> CreateMessagePublishSubscribeChannelWith3SubScribers()
        {
            IPublishSubscribeChannel<Models.Message> target = CreatePublishSubscribeChannel<Models.Message>()
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriber<Models.Message>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriberXXX<Models.Message>)).WithTimeToExpire(new TimeSpan(0, 0, 30))
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriber2<Models.Message>)).WithTimeToExpire(new TimeSpan(0, 0, 30));
            return target;
        }

        public static PublishSubscribeChannel<T> CreatePublishSubscribeChannel<T>()
        {
            var queue = CreateMSMQQueueProvider<T>();

            TestHelper.SetUpCleanTestQueue(queue.Name);

            queue.SetupWatchQueue(queue);

            var target = new PublishSubscribeChannel<T>(queue);

            return target;
        }

        public static IQueueProvider<T> CreateMSMQQueueProvider<T>()
        {
            // TODO: Instantiate an appropriate concrete class.
            //GenericParameterHelper param = new GenericParameterHelper();
            IQueueProvider<T> target = new MsmqQueueProvider<T>();
            return target;
        }
    }

    public class TestSubscriber<T> : Subscriber<T>
    {
        #region ISubscriber Members
                public override bool Process(T input)
        {
            System.Diagnostics.Debug.WriteLine("Writing stuff: {0}", "");
            return true;
        }

        #endregion
    }
}
