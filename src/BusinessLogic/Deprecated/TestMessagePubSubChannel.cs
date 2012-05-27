//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Phantom.PubSub;
//using Entities;

//namespace BusinessLogic
//{
//    // Should get rid of this test this is not how to implement need to specify type when inheriting from channel
//    public class TestMessagePubSubChannel<T> : PublishSubscribeChannel<T>
//    {
//        public TestMessagePubSubChannel(IQueueProvider<T> queue) : base(queue)
//        {

//        }

        

//        public ISubscriber<T> GetSubscription(string subscriberName)
//        {
//            ISubscriber<T> returnSubscriber;
           
//            switch (subscriberName)
//            {
//                case "jackie":
//                    returnSubscriber = new TestMessageSubscriberUserType(new MessageService()) as ISubscriber<T>;
//                    returnSubscriber.Name = "jackie";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                case "Rube":
//                    returnSubscriber = new TestMessageSubscriberUserType2(new MessageService2()) as ISubscriber<T>;
//                    returnSubscriber.Name = "Rube";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                case "Sid":
//                    returnSubscriber = new TestMessageSubscriberUserType3(new MessageService3()) as ISubscriber<T>;
//                    returnSubscriber.Name = "Sid";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                default:
//                    returnSubscriber = new TestMessageSubscriberUserType(new MessageService()) as ISubscriber<T>;
//                    returnSubscriber.Name = "jackie";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//            }
//            return returnSubscriber;
//        }

//        public override ISubscriber<T> GetSubscription(Phantom.PubSub.IMessageStatus<T> subscriber)
//        {
//            return GetSubscription(subscriber.Name);
//        }

//        public override MessageStatus<T> GetMessageStatusTrackers()
//        {
//            try
//            {
//                //MessageService service = new MessageService();
//                //read config and creat list of subscribers

//                var subscribersStatus = new MessageStatus<T>();
//                IMessageStatus<T> sub = (IMessageStatus<T>)new TestMessageSubscriberUserType(new MessageService());
//                sub.Name = "jackie";
//               // sub.TimeToExpire = new TimeSpan(10000);
//                var sub2 = (IMessageStatus<T>)new TestMessageSubscriberUserType2(new MessageService2());
//                sub2.Name = "Rube";
//               // sub2.TimeToExpire = new TimeSpan(10000);
//                IMessageStatus<T> sub3 = (IMessageStatus<T>)new TestMessageSubscriberUserType3(new MessageService3());
//                sub3.Name = "Sid";
//              //  sub3.TimeToExpire = new TimeSpan(10000);

//                subscribersStatus.Add(sub);
//                subscribersStatus.Add(sub2);
//                subscribersStatus.Add(sub3);
//                return base.GetSubScriberStatuses(subscribersStatus);
//            }
//            catch (Exception exp)
//            {
//                System.Diagnostics.Debug.WriteLine(exp.ToString());
//                throw;
//            }
//        }
//    }
//}
