//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Phantom.PubSub;
//using Entities;
//using Phantom.PubSub;

//namespace BusinessLogic
//{
//    public class MessagePublishSubscribeChannel : PublishSubscribeChannel<User>
//    {
//        public MessagePublishSubscribeChannel(IQueueProvider<User> queue)
//            : base(queue)
//        {
//        }

//        //public override Subscribers<User> GetSubscribers()
//        //{
//        //    //read config and creat list of subscribers
//        //    var subscribers = new Subscribers<User>();
//        //    ISubscriber<User> sub = new TestSubscriber<User>() as ISubscriber<User>;
//        //    sub.Name = "jackie";
//        //    sub.TimeToExpire = new TimeSpan(10000);
//        //    ISubscriber<User> sub2 = new TestSubscriber<User>() as ISubscriber<User>;
//        //    sub2.Name = "Rube";
//        //    sub2.TimeToExpire = new TimeSpan(-10);
//        //    ISubscriber<User> sub3 = new TestSubscriber<User>() as ISubscriber<User>;
//        //    sub3.Name = "Sid";
//        //    sub3.TimeToExpire = new TimeSpan(10000);

//        //    subscribers.Add(sub);
//        //    subscribers.Add(sub2);
//        //    subscribers.Add(sub3);
//        //    //this.Subscribers = subscribers;
//        //    //this.ActiveSubscriptions = new List<ISubscriber<User>>();
//        //    //this.ActiveSubscriptions.Add(new TestSubscriber<User>());
//        //    return base.GetSubscribers(subscribers);
//        //}

//        private ISubscriber<User> GetSubscription(string subscriberName)
//        {
//            //should probably use configuration to get list and 
//            ISubscriber<User> returnSubscriber;
//            switch (subscriberName)
//            {
//                case "jackie":
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




//        public override ISubscriber<User> GetSubscription(IMessageStatus<User> MessageStatus)
//        {
//            return GetSubscription(MessageStatus.Name);
//        }

//        /// <summary>
//        /// This overriden function is where you sould instantiate the objects that contain the 
//        /// meddage status
//        /// </summary>
//        /// <returns></returns>
//        public override MessageStatus<User> GetMessageStatusTrackers()
//        {
//            //read config and creat list of subscribers
//            var MessageStatusTracker = new MessageStatus<User>();
//            IMessageStatus<User> sub = new TestSubscriber<User>() as IMessageStatus<User>;
//            sub.Name = "jackie";
//           // sub.TimeToExpire = new TimeSpan(10000);
//            IMessageStatus<User> sub2 = new TestSubscriber<User>() as IMessageStatus<User>;
//            sub2.Name = "Rube";
//            //sub2.TimeToExpire = new TimeSpan(-10);
//            IMessageStatus<User> sub3 = new TestSubscriber<User>() as IMessageStatus<User>;
//            sub3.Name = "Sid";
//           // sub3.TimeToExpire = new TimeSpan(10000);

//            MessageStatusTracker.Add(sub);
//            MessageStatusTracker.Add(sub2);
//            MessageStatusTracker.Add(sub3);
//            //this.Subscribers = subscribers;
//            //this.ActiveSubscriptions = new List<ISubscriber<User>>();
//            //this.ActiveSubscriptions.Add(new TestSubscriber<User>());
//            return base.GetSubScriberStatuses(MessageStatusTracker);
//        }
//    }
//}
