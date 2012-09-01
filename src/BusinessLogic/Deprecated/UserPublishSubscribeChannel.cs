//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Phantom.PubSub;
//using Entities;

//namespace BusinessLogic
//{
//    public class UserPublishSubscribeChannel : PublishSubscribeChannel<User>
//    {
//        public UserPublishSubscribeChannel(IQueueProvider<User>queue):base(queue) 
//        {
//        }

//        private ISubscriber<User> GetSubscription(string subscriberName)
//        {
//            //should probably use configuration to get list and 
//            ISubscriber<User> returnSubscriber;
//            switch (subscriberName)
//            {
//                case  "jackie":
//                    returnSubscriber = new TestSubscriber<User>() as ISubscriber<User>;
//                    returnSubscriber.Name = "jackie";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                case "Rube":
//                    returnSubscriber = new TestSubscriber<User>() as ISubscriber<User>;
//                    returnSubscriber.Name = "Rube";
//                    returnSubscriber.TimeToExpire = new TimeSpan(-10);
//                    break;
//                case "Sid":
//                    returnSubscriber = new TestSubscriber<User>() as ISubscriber<User>;
//                    returnSubscriber.Name = "Sid";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//                default:
//                    returnSubscriber = new TestSubscriber<User>() as ISubscriber<User>;
//                    returnSubscriber.Name = "jackie";
//                    returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                    break;
//            }
//            return returnSubscriber;
//        }

//        public override ISubscriber<User> GetSubscription(IMessageStatus<User> subscriber)
//        {
//            return GetSubscription(subscriber.Name);
//        }

//        public override MessageStatus<User> GetMessageStatusTrackers()
//        {
//            var MessageStatusTracker = new MessageStatus<User>();

//            IMessageStatus<User> sub = new TestSubscriber<User>() as IMessageStatus<User>;
//            sub.Name = "jackie";
//           // sub.TimeToExpire = new TimeSpan(10000);
//            IMessageStatus<User> sub2 = new TestSubscriber<User>() as IMessageStatus<User>;
//            sub2.Name = "Rube";
//           // sub2.TimeToExpire = new TimeSpan(-10);
//            IMessageStatus<User> sub3 = new TestSubscriber<User>() as IMessageStatus<User>;
//            sub3.Name = "Sid";
//           // sub3.TimeToExpire = new TimeSpan(10000);

//            MessageStatusTracker.Add(sub);
//            MessageStatusTracker.Add(sub2);
//            MessageStatusTracker.Add(sub3);
//            ////this.Subscribers = subscribers;
//            ////this.ActiveSubscriptionsDictionary = new List<ISubscriber<User>>();
//            ////this.ActiveSubscriptionsDictionary.Add(new TestSubscriber<User>());
//            return base.GetSubScriberStatuses(MessageStatusTracker); 
//        }
//    }
//}
