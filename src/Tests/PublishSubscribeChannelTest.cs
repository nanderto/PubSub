using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Entities;
using Phantom.PubSub;
using System.Threading;
using Phantom.PubSub.Fakes;
using System.Collections.Generic;
using System.Diagnostics;

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

        [TestMethod, TestCategory("UnitTest")]
        public void AddSubscriberType_Without_WithTimeToExpire_WithFakeProvider()
        {
            var subscriberInfo = new PublishSubscribeChannel<User>(new StubIStoreProvider<User>())
                .AddSubscriberType(typeof(TestSubscriberZZZ<User>));
            var pubsub = subscriberInfo.WithTimeToExpire(new TimeSpan(0, 1, 0));
            Assert.IsInstanceOfType(pubsub, typeof(PublishSubscribeChannel<User>));

            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannelConstructor_WithFakeProvider()
        {
            var pubsub = new PublishSubscribeChannel<User>(new StubIStoreProvider<User>())
                .AddSubscriberType(typeof(TestSubscriberZZZ<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            Assert.IsInstanceOfType(pubsub, typeof(PublishSubscribeChannel<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));

        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannelConstructor_Add2Subscribers_WithFakeProvider()
        {
            var pubsub = new PublishSubscribeChannel<User>(new StubIStoreProvider<User>())
                .AddSubscriberType(typeof(TestSubscriberZZZ<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(TestSubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            Assert.AreEqual(2, pubsub.GetSubscriptions().Count);
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[1], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(TestSubscriberZZZ<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[1], typeof(TestSubscriber2<User>));

        }

        [TestMethod, TestCategory("UnitTest")]
        public void PublishSubscribeChannelConstructor_Add3Subscribers_WithFakeProvider()
        {
            var pubsub = new PublishSubscribeChannel<User>(new StubIStoreProvider<User>())
                .AddSubscriberType(typeof(TestSubscriberZZZ<User>)).WithTimeToExpire(new TimeSpan(0,1,0))
                .AddSubscriberType(typeof(TestSubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(TestSubscriber3<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            Assert.AreEqual(3, pubsub.GetSubscriptions().Count);
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[0], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[1], typeof(ISubscriber<User>));
            Assert.IsInstanceOfType(pubsub.GetSubscriptions()[2], typeof(TestSubscriber3<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Get_1_Subscribers_WithFakeProvider()
        {
            var pubsub = new PublishSubscribeChannel<User>(new StubIStoreProvider<User>())
             .AddSubscriberType(typeof(TestSubscriberZZZ<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            var Subscribers = pubsub.GetSubscriptions();
            Assert.IsInstanceOfType(Subscribers[0], typeof(TestSubscriberZZZ<User>));
            Assert.AreEqual(Subscribers[0].Name, "TestSubscriberZZZ`1");
            Assert.AreEqual(Subscribers[0].TimeToExpire, new TimeSpan(0, 1, 0));
            Assert.IsTrue(Subscribers.Count == 1);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Get_2_Subscribers_WithFakeProvider()
        {
            var pubsub = new PublishSubscribeChannel<User>(new StubIStoreProvider<User>())
             .AddSubscriberType(typeof(TestSubscriberZZZ<User>)).WithTimeToExpire(new TimeSpan(0, 1, 1))
             .AddSubscriberType(typeof(TestSubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 0, 1));
            var Subscribers = pubsub.GetSubscriptions();
            Assert.IsInstanceOfType(Subscribers[0], typeof(TestSubscriberZZZ<User>));
            Assert.IsInstanceOfType(Subscribers[1], typeof(TestSubscriber2<User>));
            
            Assert.AreEqual(Subscribers[0].Name, "TestSubscriberZZZ`1");
            Assert.AreEqual(Subscribers[0].TimeToExpire, new TimeSpan(0, 1, 1));
            
            Assert.AreEqual(Subscribers[1].Name, "TestSubscriber2`1");
            Assert.AreEqual(Subscribers[1].TimeToExpire, new TimeSpan(0, 0, 1));

            Assert.IsTrue(Subscribers.Count == 2);
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void ProcessBatch_Subscriberfails_WithFakeStoreProvider()
        {
            TestUtils.TestHelper.autoEvent = new AutoResetEvent(false);

            var m = new Message();

            var messagePacket = TestUtils.TestHelper.BuildAMessage<Message>(m)
                .WithSubscriberMetadataFor(typeof(TestUtils.SpeedySubscriberGuaranteedExceptions<Message>), false, false).GetMessage();
            var beforeRetryCount = messagePacket.SubscriberMetadataList[0].RetryCount;

            MessagePacket<Message> outputPostProcessMP = default(MessagePacket<Message>);
            string outputMessageId = string.Empty;

            var MessagePubSubChannel = new PublishSubscribeChannel<Message>();

            var stubStoreProvider = new StubIStoreProvider<Message>
            {
                ProcessStoreAsBatchFuncOfMessagePacketOfT0StringBoolean = funct => MessagePubSubChannel.HandleMessageForBatchProcessing(messagePacket, messagePacket.MessageId.ToString()),
                UpdateMessageStoreMessagePacketOfT0 = UpdatedMessagepacket =>
                {
                    outputPostProcessMP = UpdatedMessagepacket;
                    TestUtils.TestHelper.autoEvent.Set();
                    return;
                },
                SubscriberGroupCompletedForMessageString = messageId =>
                {
                    Assert.IsTrue(messagePacket.MessageId.ToString() == messageId);
                    
                    return true;
                }
            };
            MessagePubSubChannel.StorageProvider = stubStoreProvider;

            MessagePubSubChannel.AddSubscriberType(typeof(TestUtils.SpeedySubscriberGuaranteedExceptions<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 0, 1));


            MessagePubSubChannel.ProcessBatch();
            TestUtils.TestHelper.autoEvent.WaitOne(); //new TimeSpan(0, 0, 0, 5, 0)
            Assert.IsTrue(outputPostProcessMP.SubscriberMetadataList[0].RetryCount == messagePacket.SubscriberMetadataList[0].RetryCount + 1);
            Assert.IsTrue(outputPostProcessMP.MessageId == messagePacket.MessageId);
            Assert.IsTrue(outputPostProcessMP.SubscriberMetadataList.Count == 1);
            Assert.IsTrue(outputPostProcessMP.SubscriberMetadataList[0].FailedOrTimedOut == true);
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void ProcessBatch_MessageTest_WithFakeStoreProvider()
        {
            TestUtils.TestHelper.autoEvent = new AutoResetEvent(false);

            var m = new Message();

            var messagePacket = TestUtils.TestHelper.BuildAMessage<Message>(m)
                .WithSubscriberMetadataFor(typeof(TestUtils.SpeedySubscriber2<Message>), false, false).GetMessage();

            MessagePacket<Message> outputPostProcessMP = default(MessagePacket<Message>);
            string outputMessageId = string.Empty;

            var MessagePubSubChannel = new PublishSubscribeChannel<Message>();

            var stubStoreProvider = new StubIStoreProvider<Message>
            {
                ProcessStoreAsBatchFuncOfMessagePacketOfT0StringBoolean = funct => MessagePubSubChannel.HandleMessageForBatchProcessing(messagePacket, messagePacket.MessageId.ToString()),
                UpdateMessageStoreMessagePacketOfT0 = UpdatedMessagepacket =>
                {
                    outputPostProcessMP = UpdatedMessagepacket;
                    TestUtils.TestHelper.autoEvent.Set();
                    return;
                },
                SubscriberGroupCompletedForMessageString = messageId =>
                {
                    Assert.IsTrue(messagePacket.MessageId.ToString() == messageId);
                    return true;
                }
            };
            MessagePubSubChannel.StorageProvider = stubStoreProvider;

            MessagePubSubChannel.AddSubscriberType(typeof(TestUtils.SpeedySubscriber2<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 0, 1))
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriber2<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 0, 1));


            MessagePubSubChannel.ProcessBatch();
            TestUtils.TestHelper.autoEvent.WaitOne(new TimeSpan(0, 0, 0, 50, 0));
            Assert.IsTrue(outputPostProcessMP.MessageId == messagePacket.MessageId);
            Assert.IsTrue(outputPostProcessMP.SubscriberMetadataList.Count == 1);
            Assert.IsTrue(outputPostProcessMP.SubscriberMetadataList[0].FailedOrTimedOut == false);
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void HandleMessageForFirstTime_MessageTest_WithFakeStoreProvider()
        {
            TestUtils.TestHelper.Subscriber1Ran = new AutoResetEvent(false);
            TestUtils.TestHelper.Subscriber2Ran = new AutoResetEvent(false);
            TestUtils.TestHelper.Subscriber3Ran = new AutoResetEvent(false);
            TestUtils.TestHelper.UpdateMessageStoreEvent = new AutoResetEvent(false);
            TestUtils.TestHelper.SubscriberGroupCompletedEvent = new AutoResetEvent(false);

            var m = new Message { Name = "MyMessage"};

            var messagePacket = TestUtils.TestHelper.BuildAMessage<Message>(m)
                .WithSubscriberMetadataFor(typeof(TestUtils.SpeedySubscriber<Message>), false, false)
                .WithSubscriberMetadataFor(typeof(TestUtils.SpeedySubscriber2<Message>), false, false).GetMessage();

            MessagePacket<Message> outputPostProcessMP = default(MessagePacket<Message>);
            string outputMessageId = string.Empty;
            DateTime timelastupdateCalled = default(DateTime);
            DateTime timeGroupCompletedCalled = default(DateTime);

            var MessagePubSubChannel = new PublishSubscribeChannel<Message>();
            var stubStoreProvider = new StubIStoreProvider<Message>
            {
                UpdateMessageStoreMessagePacketOfT0 = UpdatedMessagepacket =>
                {
                    outputPostProcessMP = UpdatedMessagepacket;
                    timelastupdateCalled = DateTime.Now;
                    TestUtils.TestHelper.UpdateMessageStoreEvent.Set();
                    return;
                },
                SubscriberGroupCompletedForMessageString = messageId =>
                {
                    Assert.IsTrue(messagePacket.MessageId.ToString() == messageId);
                    timeGroupCompletedCalled = DateTime.Now;
                    TestUtils.TestHelper.SubscriberGroupCompletedEvent.Set();
                    return true;
                }
            };
            MessagePubSubChannel.StorageProvider = stubStoreProvider;

            MessagePubSubChannel.AddSubscriberType(typeof(TestUtils.SpeedySubscriber2<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 1, 1))
                .AddSubscriberType(typeof(TestUtils.SpeedySubscriber<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 1, 1));


            MessagePubSubChannel.HandleMessageForFirstTime(messagePacket, messagePacket.MessageId.ToString());

            TestUtils.TestHelper.Subscriber1Ran.WaitOne();
            TestUtils.TestHelper.Subscriber2Ran.WaitOne();
            TestUtils.TestHelper.UpdateMessageStoreEvent.WaitOne();
            TestUtils.TestHelper.SubscriberGroupCompletedEvent.WaitOne();
            
            Assert.IsTrue(outputPostProcessMP.MessageId == messagePacket.MessageId);
            Assert.IsTrue(outputPostProcessMP.SubscriberMetadataList.Count == 1);
            Assert.IsTrue(outputPostProcessMP.SubscriberMetadataList[0].FailedOrTimedOut == false);
            Assert.IsTrue(DateTime.Compare(timelastupdateCalled, timeGroupCompletedCalled) < 0 || DateTime.Compare(timelastupdateCalled, timeGroupCompletedCalled) == 0);
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void HandleMessageForFirstTime_ExceptionThrownMessageTest_WithFakeStoreProvider()
        {
            TestUtils.TestHelper.Subscriber1Ran = new AutoResetEvent(false);
            TestUtils.TestHelper.Subscriber2Ran = new AutoResetEvent(false);
            TestUtils.TestHelper.Subscriber3Ran = new AutoResetEvent(false);
            TestUtils.TestHelper.UpdateMessageStoreEvent = new AutoResetEvent(false);
            TestUtils.TestHelper.SubscriberGroupCompletedEvent = new AutoResetEvent(false);

            var m = new Message { Name = "MyMessage" };

            var messagePacket = TestUtils.TestHelper.BuildAMessage<Message>(m)
                .WithSubscriberMetadataFor(typeof(TestUtils.SpeedySubscriber<Message>), false, false)
                .WithSubscriberMetadataFor(typeof(TestUtils.SpeedySubscriberGuaranteedExceptions<Message>), false, false).GetMessage();

            List<MessagePacket<Message>> outputPostProcessMP = new List<MessagePacket<Message>>();
            string outputMessageId = string.Empty;
            DateTime timelastupdateCalled = default(DateTime);
            DateTime timeGroupCompletedCalled = default(DateTime);

            var MessagePubSubChannel = new PublishSubscribeChannel<Message>();
            var stubStoreProvider = new StubIStoreProvider<Message>
            {
                UpdateMessageStoreMessagePacketOfT0 = UpdatedMessagepacket =>
                {
                    outputPostProcessMP.Add(UpdatedMessagepacket);
                    timelastupdateCalled = DateTime.Now;
                    TestUtils.TestHelper.UpdateMessageStoreEvent.Set();
                    return;
                },
                SubscriberGroupCompletedForMessageString = messageId =>
                {
                    Assert.IsTrue(messagePacket.MessageId.ToString() == messageId);
                    timeGroupCompletedCalled = DateTime.Now;
                    TestUtils.TestHelper.SubscriberGroupCompletedEvent.Set();
                    return true;
                },
            };

            MessagePubSubChannel.StorageProvider = stubStoreProvider;

            MessagePubSubChannel.AddSubscriberType(typeof(TestUtils.SpeedySubscriberGuaranteedExceptions<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 1, 1))
                .AddSubscriberType(typeof(TestUtils.SpeedySubscriber<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 1, 1));


            MessagePubSubChannel.HandleMessageForFirstTime(messagePacket, messagePacket.MessageId.ToString());

            TestUtils.TestHelper.Subscriber1Ran.WaitOne();
            TestUtils.TestHelper.Subscriber3Ran.WaitOne();
            TestUtils.TestHelper.UpdateMessageStoreEvent.WaitOne();
            TestUtils.TestHelper.SubscriberGroupCompletedEvent.WaitOne();

            foreach (var item in outputPostProcessMP)
            {
                Assert.IsTrue(item.MessageId == messagePacket.MessageId);
                Assert.IsTrue(item.SubscriberMetadataList.Count == 1);
                if (item.SubscriberMetadataList[0].Name.Contains("SpeedySubscriberGuaranteedExceptions"))
                {
                    Assert.IsTrue(item.SubscriberMetadataList[0].FailedOrTimedOut == true);
                }
                else 
                {
                    Assert.IsTrue(item.SubscriberMetadataList[0].FailedOrTimedOut == false);
                }
            }
            Trace.WriteLine("timelastupdateCalled: " + timelastupdateCalled.TimeOfDay);
            Trace.WriteLine("timeGroupCompletedCa: " + timeGroupCompletedCalled.TimeOfDay);
            Assert.IsTrue(DateTime.Compare(timelastupdateCalled, timeGroupCompletedCalled) < 0 || DateTime.Compare(timelastupdateCalled, timeGroupCompletedCalled) == 0);

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
