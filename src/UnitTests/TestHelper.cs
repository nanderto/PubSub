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
            MessageQueue.Create(@".\private$\EntitiesUser", true);

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUserPoisonMessages"))
                MessageQueue.Delete(@".\private$\EntitiesUserPoisonMessages");
            MessageQueue.Create(@".\private$\EntitiesUserPoisonMessages", true);
        }

        public static void SetUpCleanTestQueue(string QueueName)
        {
            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\" + QueueName))
                MessageQueue.Delete(@".\private$\" + QueueName);
            MessageQueue.Create(@".\private$\" + QueueName, true);

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\" + QueueName + "PoisonMessages"))
                MessageQueue.Delete(@".\private$\" + QueueName + "PoisonMessages");
            MessageQueue.Create(@".\private$\" + QueueName + "PoisonMessages", true);
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
        

        public static string AddAMessage()
        {
            Models.User u = new Models.User();
            u.FirstName = "Gwen";
            u.LastName = "XXXX";

            Message recoverableMessage = new Message();
            recoverableMessage.Body = u;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            MessageQueueTransaction msgTx = new MessageQueueTransaction();
            msgTx.Begin();
            try
            {
                msgQ.Send(recoverableMessage, msgTx);
            }
            catch (Exception ex)
            {
                msgTx.Abort();
                System.Diagnostics.Trace.WriteLine("Exception::::::: " + ex.ToString());
            }
            finally
            {
                msgTx.Commit();
                
            }
            return recoverableMessage.Id;
        }

        public static string AddAMessage(string QueueName)
        {
            Entities.Message m = new Entities.Message();
            m.Name= "Gwen";

            Message recoverableMessage = new Message();
            recoverableMessage.Body = m;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\" + QueueName);
            MessageQueueTransaction msgTx = new MessageQueueTransaction();
            msgTx.Begin();
            try
            {
                msgQ.Send(recoverableMessage, msgTx);
            }
            catch (Exception ex)
            {
                msgTx.Abort();
                System.Diagnostics.Trace.WriteLine("Exception::::::: " + ex.ToString());
            }
            finally
            {
                msgTx.Commit();
                
            }
            return recoverableMessage.ToString();
        }
        public static List<IMessageStatus<T>> GetSubscriptionsStatuses<T>()
        {
            var subscribers = new List<IMessageStatus<T>>();
            IMessageStatus<T> sub = new TestSubscriber<T>() as IMessageStatus<T>;
            sub.Name = "jackie";
            sub.Id = "SubscriptionID::jackie::1";
           // sub.TimeToExpire = new TimeSpan(0, 0, 2);
            IMessageStatus<T> sub2 = new TestSubscriber<T>() as IMessageStatus<T>;
            sub2.Name = "Rube";
            sub2.Id = "SubscriptionID::Rube::2";
           // sub2.TimeToExpire = new TimeSpan(0, 0, 20);
            IMessageStatus<T> sub3 = new TestSubscriber<T>() as IMessageStatus<T>;
            sub3.Name = "Sid";
            sub3.Id = "SubscriptionID::Sid::3";
           // sub3.TimeToExpire = new TimeSpan(0, 1, 20);

            subscribers.Add(sub);
            subscribers.Add(sub2);
            subscribers.Add(sub3);
            return subscribers;
        }

        public static List<Phantom.PubSub.ISubscriber<T>> GetSubscribers<T>()
        {
            var subscribers = new List<ISubscriber<T>>();
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

        public static List<IMessageStatus<T>> GetSubscribersStatuses<T>()
        {
            var subscribers = new List<IMessageStatus<T>>();
            IMessageStatus<T> sub = new TestSubscriber<T>() as IMessageStatus<T>;
            sub.Name = "jackie";
            sub.Id = "SubscriptionID::jackie::1";
            //sub.TimeToExpire = new TimeSpan(0, 0, 2);
            IMessageStatus<T> sub2 = new TestSubscriber<T>() as IMessageStatus<T>;
            sub2.Name = "Rube";
            sub2.Id = "SubscriptionID::Rube::2";
           // sub2.TimeToExpire = new TimeSpan(0, 0, 20);
            IMessageStatus<T> sub3 = new TestSubscriber<T>() as IMessageStatus<T>;
            sub3.Name = "Sid";
            sub3.Id = "SubscriptionID::Sid::3";
          //  sub3.TimeToExpire = new TimeSpan(0, 1, 20);

            subscribers.Add(sub);
            subscribers.Add(sub2);
            subscribers.Add(sub3);
            return subscribers;
        }

        private static Phantom.PubSub.ISubscriber<T> GetSubscriber<T>(Phantom.PubSub.IMessageStatus<T> item)
        {
            ISubscriber<T> returnSubscriber;

            switch (item.Name)
            {
                case "jackie":
                    returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
                    returnSubscriber.Name = "jackie";
                    returnSubscriber.TimeToExpire = new TimeSpan(0, 0, 2);
                    returnSubscriber.MessageStatusTracker = item;
                    returnSubscriber.Id = item.Id;
                    break;
                case "Rube":
                    returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
                    returnSubscriber.Name = "Rube";
                    returnSubscriber.TimeToExpire = new TimeSpan(0, 0, 2);
                    returnSubscriber.MessageStatusTracker = item;
                    returnSubscriber.Id = item.Id;
                    break;
                case "Sid":
                    returnSubscriber = new TestSubscriber<T>()  as ISubscriber<T>;
                    returnSubscriber.Name = "Sid";
                    returnSubscriber.TimeToExpire = new TimeSpan(0, 0, 2);
                    returnSubscriber.MessageStatusTracker = item;
                    returnSubscriber.Id = item.Id;
                    break;
                case "XXX":
                    returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
                    returnSubscriber.Name = "XXX";
                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
                    returnSubscriber.MessageStatusTracker = item;
                    returnSubscriber.Id = item.Id;
                    break;
                case "YYY":
                    returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
                    returnSubscriber.Name = "YYY";
                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
                    returnSubscriber.MessageStatusTracker = item;
                    returnSubscriber.Id = item.Id;
                    break;
                case "ZZZ":
                    returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
                    returnSubscriber.Name = "ZZZ";
                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
                    returnSubscriber.MessageStatusTracker = item;
                    returnSubscriber.Id = item.Id;
                    break;
                default:
                    returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
                    returnSubscriber.Name = "jackie";
                    returnSubscriber.TimeToExpire = new TimeSpan(0, 0, 2);
                    returnSubscriber.MessageStatusTracker = item;
                    returnSubscriber.Id = item.Id;
                    break;
            }
            return returnSubscriber;
        }
        public static ActiveSubscriptions<T> AddSubscribers<T> (ActiveSubscriptions<T> input, List<ISubscriber<T>> CollecctionToAdd)
        {
            foreach (var item in CollecctionToAdd)
            {
                input.AddActiveSubscription(item);
            }
            return input;
        }

        public static ActiveSubscriptions<T> GetSpecialSubscribers<T>()
        {
            var subscribers = new ActiveSubscriptions<T>();
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

        //public static ActiveSubscriptions<T> GetSpecialSubscribersStatuses<T>()
        //{
        //    var subscribers = new ActiveSubscriptions<T>();
        //    ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
        //    sub.Name = "XXX";
        //    sub.TimeToExpire = new TimeSpan(10000);
        //    sub.Id = "Subdcription ID::XXX";
        //    ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
        //    sub2.Name = "YYY";
        //    sub2.TimeToExpire = new TimeSpan(10000);
        //    sub2.Id = "Subdcription ID::YYY";
        //    ISubscriber<T> sub3 = new TestSubscriber<T>() as ISubscriber<T>;
        //    sub3.Name = "ZZZ";
        //    sub3.TimeToExpire = new TimeSpan(10000);
        //    sub3.Id = "Subdcription ID::ZZZ";
        //    subscribers.AddActiveSubscription(sub);
        //    subscribers.AddActiveSubscription(sub2);
        //    subscribers.AddActiveSubscription(sub3);
        //    return subscribers;
        //}

        public static ActiveSubscriptions<T> AddAllotofSubscriptions<T>(ActiveSubscriptions<T> input, int NumberToAdd)
        {
            //AddTest a bunch of active subscriptions
            int i = 0;
            for (int j = 0; j < NumberToAdd; j++)
            {

                foreach (IMessageStatus<T> item in TestHelper.GetSubscriptionsStatuses<T>())
                {
                    item.Id = " SubScription: " + item.Name + j + ":: MessageID: " + i + "000::";
                   // item.MessageStatusTracker = new MessageStatusTracker();
                    input.AddActiveSubscription(GetSubscriber<T>(item));
                    i++;
                }
            }
            return input;
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

            Message recoverableMessage = new Message();
            recoverableMessage.Body = m;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            MessageQueueTransaction msgTx = new MessageQueueTransaction();
            msgTx.Begin();
            try
            {
                msgQ.Send(recoverableMessage, msgTx);
            }
            catch (Exception ex)
            {
                msgTx.Abort();
                System.Diagnostics.Trace.WriteLine("Exception::::::: " + ex.ToString());
            }
            finally
            {
                msgTx.Commit();

            }
            return recoverableMessage.Id;
        }

        public static string AddAMessageMessage(string QueueName)
        {
            Models.Message m = new Models.Message();
            m.Name = "Gwen";
            m.BatchNumber = 1;
            m.Guid = System.Guid.NewGuid();
            m.MessageID = "MessageID";
            m.SubscriptionID = "SubscriptionID";
            m.MessagePutTime = DateTime.Now;

            Message recoverableMessage = new Message();
            recoverableMessage.Body = m;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\" + QueueName);
            MessageQueueTransaction msgTx = new MessageQueueTransaction();
            msgTx.Begin();
            try
            {
                msgQ.Send(recoverableMessage, msgTx);
            }
            catch (Exception ex)
            {
                msgTx.Abort();
                System.Diagnostics.Trace.WriteLine("Exception::::::: " + ex.ToString());
            }
            finally
            {
                msgTx.Commit();

            }
            return recoverableMessage.Id;
        }
    }
}
