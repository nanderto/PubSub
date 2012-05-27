//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Phantom.PubSub;
//using Entities;

//namespace BusinessLogic
//{
//    public class TestMessagePubSubChannelGeneric : PublishSubscribeChannel<Message>
//    {
//        public TestMessagePubSubChannelGeneric(IQueueProvider<Message> queue)
//            : base(queue)
//        {

//        }

//        /// <summary>
//        /// This method needs to return a new instance of the specific subscriber that it is asked for
//        /// It will be used to provcess a new message
//        /// </summary>
//        /// <param name="subscriberName">Name of the subscriber that you need</param>
//        /// <returns>A new instance of the specified subscriber</returns>
//        public ISubscriber<Message> GetSubscription(string subscriberName)
//        {
//            ISubscriber<Message> returnSubscriber;

//            switch (subscriberName)
//            {
//                case "jackie":
//                    returnSubscriber = new TestMessageSubscriberGenericType<Message>(new MessageService()) as ISubscriber<Message>;
//                    returnSubscriber.Name = "jackie";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                case "Rube":
//                    returnSubscriber = new TestMessageSubscriberGenericType2<Message>(new MessageService2()) as ISubscriber<Message>;
//                    returnSubscriber.Name = "Rube";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                case "Sid":
//                    returnSubscriber = new TestMessageSubscriberGenericType3<Message>(new MessageService3()) as ISubscriber<Message>;
//                    returnSubscriber.Name = "Sid";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                default:
//                    returnSubscriber = new TestMessageSubscriberGenericType<Message>(new MessageService()) as ISubscriber<Message>;
//                    returnSubscriber.Name = "jackie";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//            }
//            return returnSubscriber;
//        }

//        public override ISubscriber<Message> GetSubscription(IMessageStatus<Message> subscriber)
//        {
//            return GetSubscription(subscriber.Name);
//        }

//        public override MessageStatus<Message> GetMessageStatusTrackers()
//        {
//            try
//            {
//                MessageService service = new MessageService();
//                //read config and creat list of subscribers

//                var MessageStatusTracker = new MessageStatus<Message>();
//                IMessageStatus<Message> sub = (IMessageStatus<Message>) new TestMessageMessageStatusTracker<Message>();
//                sub.Name = "jackie";
//                //sub.TimeToExpire = new TimeSpan(10000);
//                var sub2 = (IMessageStatus<Message>)new TestMessageMessageStatusTracker2<Message>();
//                sub2.Name = "Rube";
//                //sub2.TimeToExpire = new TimeSpan(10000);
//                IMessageStatus<Message> sub3 = (IMessageStatus<Message>)new TestMessageMessageStatusTracker3<Message>();
//                sub3.Name = "Sid";
//                //sub3.TimeToExpire = new TimeSpan(10000);

//                MessageStatusTracker.Add(sub);
//                MessageStatusTracker.Add(sub2);
//                MessageStatusTracker.Add(sub3);
//                return base.GetSubScriberStatuses(MessageStatusTracker);
//            }
//            catch (Exception exp)
//            {
//                System.Diagnostics.Debug.WriteLine(exp.ToString());
//                throw;
//            }
//        }
//    }
//}
