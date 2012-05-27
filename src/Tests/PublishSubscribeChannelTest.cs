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
            Assert.IsInstanceOfType(target.ActiveSubscriptions, typeof(ActiveSubscriptions<User>), "did not create the correct type");
            
        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannelConstructor()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
                .AddSubscriber(typeof(TestSubscriber<User>));
            Assert.IsInstanceOfType(pubsub, typeof(PublishSubscribeChannel<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Get_1_MessageStatusTrackers()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
             .AddSubscriber(typeof(TestSubscriber<User>));
            var MessageStatusTrackers = pubsub.GetMessageStatusTrackers();
            Assert.IsInstanceOfType(MessageStatusTrackers, typeof(MessageStatus<User>));
            Assert.IsTrue(MessageStatusTrackers.Count == 1);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Get_2_MessageStatusTrackers()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
                .AddSubscriber(typeof(BusinessLogic.TestSubscriber<>))
                .AddSubscriber(typeof(BusinessLogic.TestSubscriber2<>));
            var MessageStatusTrackers = pubsub.GetMessageStatusTrackers();
            Assert.IsInstanceOfType(MessageStatusTrackers, typeof(MessageStatus<User>));
            Assert.IsTrue(MessageStatusTrackers.Count == 2);

            foreach (var item in MessageStatusTrackers)
            {
                Assert.IsInstanceOfType(item, typeof(IMessageStatus<User>));
                Assert.IsInstanceOfType(item, typeof(MessageStatusTracker<User>));
            }

            Assert.IsTrue(MessageStatusTrackers[0].Name == typeof(BusinessLogic.TestSubscriber<>).Name);
            Assert.IsTrue(MessageStatusTrackers[1].Name == typeof(BusinessLogic.TestSubscriber2<>).Name);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void GetSubscriptionByIMessageStatus()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqQueueProvider<User>())
                .AddSubscriber(typeof(BusinessLogic.TestSubscriber<User>))
                .AddSubscriber(typeof(BusinessLogic.TestSubscriber2<User>));

            var mst = new MessageStatusTracker<User>() as IMessageStatus<User>;
            mst.Name = typeof(BusinessLogic.TestSubscriber<User>).Name;
            var subscriber = pubsub.GetSubscription(mst);
            Assert.IsInstanceOfType(subscriber, typeof(BusinessLogic.TestSubscriber<User>));

            var mst2 = new MessageStatusTracker<User>() as IMessageStatus<User>;
            mst2.Name = typeof(BusinessLogic.TestSubscriber2<User>).Name;
            var subscriber2 = pubsub.GetSubscription(mst2);
            Assert.IsInstanceOfType(subscriber2, typeof(BusinessLogic.TestSubscriber2<User>));
        }
    }
}
