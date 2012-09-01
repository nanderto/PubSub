using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Entities;
using Phantom.PubSub;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for PublishSubscribeChannelTest and is intended
    ///to contain all PublishSubscribeChannelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PublishSubscribeChannelTest
    {
       

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for PublishSubscribeChannel Constructor
        ///</summary>
        [TestCategory("UnitTest"), TestMethod()]
        public void PublishSubscribeChannelConstructorTest()
        {
            IQueueProvider<User> Queue = new MsmqQueueProvider<User>() as IQueueProvider<User>;
            var target = new PublishSubscribeChannel<User>(Queue);
            Assert.IsInstanceOfType(Queue, typeof(IQueueProvider<User>), "did not create the correct type");
            Assert.IsInstanceOfType(target, typeof(PublishSubscribeChannel<User>), "did not create the correct type");
            //Assert.IsInstanceOfType(PublishSubscribeChannel<User>.ActiveSubscriptions, typeof(ActiveSubscriptionsDictionary<User>), "did not create the correct type");          
        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannel_ProcessCompleted()
        {
            //MessageStatusTrackers<User> statustrackers = null;
            //IMessageStatus<User> MessageStatustracker = null;
            //ISubscriber<User> Subscriber = null;

            //PublishSubscribeChannel<User> pubsub = (PublishSubscribeChannel<User>)new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
            //    .AddSubscriberType(typeof(TestSubscriber<User>));
            //pubsub.ProcessCompleted("MessageId", "SubscriberId", Subscriber, MessageStatustracker, statustrackers);
            //Assert.IsInstanceOfType(pubsub, typeof(PublishSubscribeChannel<User>));
            //Assert.IsInstanceOfType(pubsub.GetMessageStatusTrackers()[0], typeof(IMessageStatus<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void AddSubscriberType_Without_WithTimeToExpire()
        {
            var subscriberInfo = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
                .AddSubscriberType(typeof(TestSubscriber<User>));
            var pubsub = subscriberInfo.WithTimeToExpire(new TimeSpan(0, 1, 0));
            Assert.IsInstanceOfType(pubsub, typeof(PublishSubscribeChannel<User>));

            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannelConstructor()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
                .AddSubscriberType(typeof(TestSubscriber<User>)).WithTimeToExpire(new TimeSpan(0,1,0));
            Assert.IsInstanceOfType(pubsub, typeof(PublishSubscribeChannel<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannelConstructor_Add2Subscribers()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
                .AddSubscriberType(typeof(TestSubscriber<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(TestSubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            Assert.AreEqual(2, pubsub.GetSubscriptions().Count);
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[1], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(TestSubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[1], typeof(TestSubscriber2<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannelConstructor_Add3Subscribers()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
                .AddSubscriberType(typeof(TestSubscriber<User>)).WithTimeToExpire(new TimeSpan(0,1,0))
                .AddSubscriberType(typeof(TestSubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(TestSubscriber3<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            Assert.AreEqual(3, pubsub.GetSubscriptions().Count);
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[1], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[2], typeof(TestSubscriber3<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Get_1_Subscribers()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
             .AddSubscriberType(typeof(TestSubscriber<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            var Subscribers = pubsub.GetSubscriptions();
            Assert.IsInstanceOfType(Subscribers[0], typeof(TestSubscriber<User>));
            Assert.AreEqual(Subscribers[0].Name, "TestSubscriber`1");
            Assert.AreEqual(Subscribers[0].TimeToExpire, new TimeSpan(0, 1, 0));
            Assert.IsTrue(Subscribers.Count == 1);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Get_2_Subscribers()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
             .AddSubscriberType(typeof(TestSubscriber<User>)).WithTimeToExpire(new TimeSpan(0, 1, 1))
             .AddSubscriberType(typeof(TestSubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 0, 1));
            var Subscribers = pubsub.GetSubscriptions();
            Assert.IsInstanceOfType(Subscribers[0], typeof(TestSubscriber<User>));
            Assert.IsInstanceOfType(Subscribers[1], typeof(TestSubscriber2<User>));
            
            Assert.AreEqual(Subscribers[0].Name, "TestSubscriber`1");
            Assert.AreEqual(Subscribers[0].TimeToExpire, new TimeSpan(0, 1, 1));
            
            Assert.AreEqual(Subscribers[1].Name, "TestSubscriber2`1");
            Assert.AreEqual(Subscribers[1].TimeToExpire, new TimeSpan(0, 0, 1));

            Assert.IsTrue(Subscribers.Count == 2);
        }
        //[TestMethod]
        //public void AddSubscriptionsToActiveSubscriptionsAcrosChannels()
        //{

        //    var pubsub1 = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>());
        //    var pubsub2 = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>());

        //    PublishSubscribeChannel<User>.ActiveSubscriptions.AddActiveSubscription(new TestSubscriber<User>() { Id = "XXX" });
        //    PublishSubscribeChannel<User>.ActiveSubscriptions.AddActiveSubscription(new TestSubscriber<User>() { Id = "ZZZ" });

        //    Assert.AreEqual(PublishSubscribeChannel<User>.ActiveSubscriptions.Count, 2);
        //    Assert.AreEqual(PublishSubscribeChannel<User>.ActiveSubscriptions.Count, 2);

        //}
    }
}
