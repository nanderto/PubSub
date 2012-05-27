using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BusinessLogic;
using Entities;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Phantom.PubSub;
using System.Threading.Tasks;
using System.Diagnostics;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for MessageReceiverTest and is intended
    ///to contain all MessageReceiverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MessageReceiverTest
    {


//        private TestContext testContextInstance;
//        private IQueueProvider<User> queue;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        // 
//        //You can use the following additional attributes as you write your tests:
//        //
//        //Use ClassInitialize to run code before running the first test in the class
//        //[ClassInitialize()]
//        //public static void MyClassInitialize(TestContext testContext)
//        //{
//        //}
//        //
//        //Use ClassCleanup to run code after all tests in a class have run
//        //[ClassCleanup()]
//        //public static void MyClassCleanup()
//        //{
//        //}
//        //
//        //Use TestInitialize to run code before running each test
//        //[TestInitialize()]
//        //public void MyTestInitialize()
//        //{
//        //}
//        //
//        //Use TestCleanup to run code after each test has run
//        //[TestCleanup()]
//        //public void MyTestCleanup()
//        //{
//        //}
//        //
//        #endregion






//        /// <summary>
//        ///A test for ProcessBatch
//        ///</summary>
//        //public void ProcessBatchTestHelper<T>()
//        //{
//        //    MessageReceiver<T> target = CreateMessageReceiver<T>(); // TODO: Initialize to an appropriate value
//        //    target.ProcessBatch();
//        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
//        //}

//        //internal virtual IMessageReceiver<T> CreateMessageReceiver<T>()
//        //{
//        //    this.queue = new UserQueueProvider();
//        //    IMessageReceiver<T> target = (IMessageReceiver<T>)new PublishSubscribeChannel<T>(IQueueProvider<T> queue);
//        //    return target;
//        //}



        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ProcessBatch_1_MessageTest()
        {
            var MessagePubSubChannel = CreateMessagePubSubChannel<Message>();
            TestHelper.AddAMessageMessage();
            MessagePubSubChannel.ProcessBatch(); 
        }

        private IPublishSubscribeChannel<Message> CreateMessagePubSubChannel<T>()
        {
            PublishSubscribeChannel<Message> target = CreateMessageReceiver<Message>();
            return target;
        }

        internal virtual PublishSubscribeChannel<Message> CreateMessageReceiver<T>()
        {
            var queue = CreateQueueProvider<Message>();
            //queue.Name = "EntitiesUser";
            TestHelper.SetUpCleanTestQueue(queue.Name);

            queue.SetUpWatchQueue(queue);

            // TODO: Instantiate an appropriate concrete class.
            //IQueueProvider<Message> queue = (IQueueProvider<Message>)new MessageQueueProvider();
            var target = new PublishSubscribeChannel<Message>(queue);

            return target;
        }

        internal virtual IQueueProvider<T> CreateQueueProvider<T>()
        {
            // TODO: Instantiate an appropriate concrete class.
            //GenericParameterHelper param = new GenericParameterHelper();
            IQueueProvider<T> target = new MsmqQueueProvider<T>();
            return target;
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ProcessBatchTest()
        {
            //ProcessBatchTestHelper<GenericParameterHelper>();

            var PublishSubscribeChannel = CreatePublishSubscribeChannel();

            PublishSubscribeChannel.ProcessBatch();

        }

        internal virtual IPublishSubscribeChannel<User> CreatePublishSubscribeChannel()
        {
            var queue = CreateQueueProvider();
            TestHelper.SetUpCleanTestQueue();
            TestHelper.AddAMessage();
            queue.SetUpWatchQueue(queue);
            var PubSubChannel = new PublishSubscribeChannel<User>(queue);

            IPublishSubscribeChannel<User> target = (IPublishSubscribeChannel<User>)PubSubChannel;
            return target;
        }


        internal virtual IQueueProvider<User> CreateQueueProvider()
        {
            // TODO: Instantiate an appropriate concrete class.
            IQueueProvider<User> target = new MsmqQueueProvider<User>() as IQueueProvider<User>;
            return target;
        }


//        internal virtual IPublishSubscribeChannel<T> CreatePublishSubscribeChannel<T>()
//        {

//            var queue = CreateQueueProvider<T>();
//            queue.Name = "EntitiesUser";
//            TestHelper.SetUpCleanTestQueue();
//            TestHelper.AddAMessageMessage();
//            queue.SetUpWatchQueue(queue);
//            var PubSubChannel = new TestMessagePubSubChannel<T>(queue);

//            IPublishSubscribeChannel<T> target = (IPublishSubscribeChannel<T>)PubSubChannel;
//            return target;
//        }

//        //public class TestQueueProvider<T> : MsmqQueueProvider<T>
//        //{
//        //    public TestQueueProvider(T type)
//        //        : base(type)
//        //    {

//        //    }

//        //    //public TestQueueProvider()
//        //    //{

//        //    //}
//        //}

//        public class TestMessageReceiver<T> : PublishSubscribeChannel<T>, IPublishSubscribeChannel<T>
//        {
//            public TestMessageReceiver(IQueueProvider<T> queue)
//                : base(queue) 
//            {
//            }

//            //public override Subscribers<T> GetSubscribers()
//            //{
//            //    //read config and creat list of subscribers
//            //    var subscribers = new Subscribers<T>();
//            //    ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
//            //    sub.Name = "jackie";
//            //    sub.TimeToExpire = new TimeSpan(10000);
//            //    ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
//            //    sub2.Name = "Rube";
//            //    sub2.TimeToExpire = new TimeSpan(10000);
//            //    ISubscriber<T> sub3 = new TestSubscriber<T>() as ISubscriber<T>;
//            //    sub3.Name = "Sid";
//            //    sub3.TimeToExpire = new TimeSpan(10000);

//            //    subscribers.Add(sub);
//            //    subscribers.Add(sub2);
//            //    subscribers.Add(sub3);
//            //    //this.Subscribers = subscribers;
//            //    //this.ActiveSubscriptions = new List<ISubscriber<T>>();
//            //    //this.ActiveSubscriptions.Add(new TestSubscriber<T>());
//            //    return base.GetSubscribers(subscribers);
//            //}

//            private ISubscriber<T> GetSubscription(string subscriberName)
//            {
//                //should probably use configuration to get list and 
//                ISubscriber<T> returnSubscriber;
//                switch (subscriberName)
//                {
//                    case "jackie":
//                        returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
//                        returnSubscriber.Name = "jackie";
//                        returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                        break;
//                    case "Rube":
//                        returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
//                        returnSubscriber.Name = "Rube";
//                        returnSubscriber.TimeToExpire = new TimeSpan(-10);
//                        break;
//                    case "Sid":
//                        returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
//                        returnSubscriber.Name = "Sid";
//                        returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                        break;
//                    default:
//                        returnSubscriber = new TestSubscriber<T>() as ISubscriber<T>;
//                        returnSubscriber.Name = "jackie";
//                        returnSubscriber.TimeToExpire = new TimeSpan(10000);
//                        break;
//                }
//                return returnSubscriber;
//            }


//            //public override ISubscriber<T> GetSubscription(ISubscriber<T> subscriber)
//            //{
//            //    return GetSubscription(subscriber.Name);
//            //}

//            public override ISubscriber<T> GetSubscription(Phantom.PubSub.IMessageStatus<T> subscriber)
//            {
//                return GetSubscription(subscriber.Name);
//            }

//            public override MessageStatus<T> GetMessageStatusTrackers()
//            {
//                var subscribers = new MessageStatus<T>();
//                IMessageStatus<T> sub = new TestSubscriber<T>() as IMessageStatus<T>;
//                sub.Name = "jackie";
//                //sub.TimeToExpire = new TimeSpan(10000);
//                IMessageStatus<T> sub2 = new TestSubscriber<T>() as IMessageStatus<T>;
//                sub2.Name = "Rube";
//               // sub2.TimeToExpire = new TimeSpan(10000);
//                IMessageStatus<T> sub3 = new TestSubscriber<T>() as IMessageStatus<T>;
//                sub3.Name = "Sid";
//               // sub3.TimeToExpire = new TimeSpan(10000);

//                subscribers.Add(sub);
//                subscribers.Add(sub2);
//                subscribers.Add(sub3);
//                //this.Subscribers = subscribers;
//                //this.ActiveSubscriptions = new List<ISubscriber<T>>();
//                //this.ActiveSubscriptions.Add(new TestSubscriber<T>());
//                return base.GetSubScriberStatuses(subscribers);
//            }
//        }
//        /// <summary>
//        ///A test for GetExpiredSubscriptions
//        ///</summary>
//        //public void GetExpiredSubscriptionsTestHelper<T>()
//        //{
//        //    MessageReceiver<T> obj = (MessageReceiver<T>)new TestMessageReceiver<T>(null);
//        //    PrivateObject param0 = new PrivateObject(obj, new PrivateType(typeof(MessageReceiver<T>))); // TODO: Initialize to an appropriate value
//        //    MessageReceiver_Accessor<T> target = new MessageReceiver_Accessor<T>(param0); // TODO: Initialize to an appropriate value
//        //    ActiveSubscriptions<T> list = new ActiveSubscriptions<T>(); 
            

//        //    ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
//        //    sub.Name = "jackie";
//        //    sub.TimeToExpire = new TimeSpan(1000000);
//        //    sub.StartTime = DateTime.Now;
//        //    ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
//        //    sub2.Name = "Rube";
//        //    sub2.TimeToExpire = new TimeSpan(-1000);
//        //    sub2.StartTime = DateTime.Now;
//        //    sub2.Aborted = true;
//        //    ISubscriber<T> sub3 = new TestSubscriber<T>() as ISubscriber<T>;
//        //    sub3.Name = "Sid";
//        //    sub3.StartTime = DateTime.Now;
//        //    sub3.TimeToExpire = new TimeSpan(1000000);
//        //    list.AddActiveSubscription(sub);
//        //    list.AddActiveSubscription(sub2);
//        //    list.AddActiveSubscription(sub3);

//        //    List<ISubscriber<T>> expected =  new List<ISubscriber<T>>();
//        //    expected.Add(sub2);

//        //    List<ISubscriber<T>> actual;
//        //    actual = target.GetExpiredSubscriptions(list);
//        //    Assert.AreEqual(expected[0].Name, actual[0].Name, " did not return the same list of aborted subscribers");
//        //}

//        //internal virtual MessageReceiver_Accessor<T> CreateMessageReceiver_Accessor<T>()
//        //{
//        //    // TODO: Instantiate an appropriate concrete class.
//        //    MessageReceiver_Accessor<T> target = null;
//        //    return target;
//        //}

        [TestCategory("Performance"), TestMethod()]
        public void StartTest()
        {
            StartTestHelper<Message>();
        }

        /// <summary>
        ///A test for Start
        ///</summary>
        public void StartTestHelper<T>()
        {

            //PublishSubscribeChannel<Message> target = CreateMessageReceiver<Message>(); // TODO: Initialize to an appropriate value
            var target = new PublishSubscribeChannel<Message>(new MsmqQueueProvider<Message>())
               .AddSubscriber(typeof(BusinessLogic.TestSubscriber<Message>))
               .AddSubscriber(typeof(BusinessLogic.TestSubscriber2<Message>));

            //target.AddSubscriber(typeof(TestMessageSubscriber<T>));
            //target.AddSubscriber(typeof(TestMessageSubscriber2<T>));

            LogEntry log = new LogEntry();
            log.Message = ("Started loading msmq: tIME:" + DateTime.Now);
            Logger.Write(log);


            for (int i = 0; i < 1000; i++)
            {
                TestHelper.AddAMessage("EntitiesMessage");
            }


            log = new LogEntry();
            log.Message = ("Started processing: tIME:" + DateTime.Now);
            Logger.Write(log);

            //PrivateObject param0 = new PrivateObject(target, new PrivateType(typeof(MessageReceiver<T>))); // TODO: Initialize to an appropriate value
            //MessageReceiver_Accessor<T> target = new MessageReceiver_Accessor<T>(param0); // TODO: Initialize to an appropriate value


            List<ISubscriber<T>> Subscribers = GetSubscribers<T>();


            target.Start();

            for (int i = 0; i < 1000; i++)
            {
                TestHelper.AddAMessage("EntitiesMessage");
            }

            DateTime start = DateTime.Now;
            while (!TestHelper.IsQueueEmpty("EntitiesMessage"))
            {
                //for (int i = 0; i < 10; i++)
                //{
                //    TestHelper.AddAMessage();
                //}

                System.Threading.Thread.Sleep(1000);
                Assert.IsTrue(DateTime.Now > start + new TimeSpan(10000), "I give up spent to much time processing");
            }

            target.Stop();

            log = new LogEntry();
            log.Message = ("Finished processing: tIME:" + DateTime.Now);
            Logger.Write(log);


            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        internal virtual List<ISubscriber<T>> GetSubscribers<T>()
        {
            List<ISubscriber<T>> list = new List<ISubscriber<T>>();

            ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
            sub.Name = "jackie";
            sub.TimeToExpire = new TimeSpan(10000);
            sub.StartTime = DateTime.Now;
            ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
            sub2.Name = "Rube";
            sub2.TimeToExpire = new TimeSpan(1000);
            sub2.StartTime = DateTime.Now;
            sub2.Aborted = true;
            ISubscriber<T> sub3 = new TestSubscriber<T>() as ISubscriber<T>;
            sub3.Name = "Sid";
            sub3.StartTime = DateTime.Now;
            sub3.TimeToExpire = new TimeSpan(10000);
            list.Add(sub);
            list.Add(sub2);
            list.Add(sub3);

            return list;
        }




        [TestCategory("Performance"), TestMethod()]
        public void StartTestMessage()
        {
            StartTestHelper2<Message>();
        }

        public void StartTestHelper2<T>()
        {

            PublishSubscribeChannel<Message> target = CreateMessageReceiver<Message>(); // TODO: Initialize to an appropriate value

            LogEntry log = new LogEntry();

            log.Message = ("Started processing: tIME:" + DateTime.Now);
            Logger.Write(log);


            for (int i = 0; i < 5000; i++)
            {
                TestHelper.AddAMessageMessage("EntitiesMessage");
            }


            //log = new LogEntry();
            var startTime = DateTime.Now;
            log.Message = ("Started loading msmq: tIME:" + startTime);
            Logger.Write(log);
            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);

            //PrivateObject param0 = new PrivateObject(target, new PrivateType(typeof(MessageReceiver<T>))); // TODO: Initialize to an appropriate value
            //MessageReceiver_Accessor<T> target = new MessageReceiver_Accessor<T>(param0); // TODO: Initialize to an appropriate value
            var queue = CreateQueueProvider<T>();
           // queue.Name = "EntitiesUser";
            //TestHelper.SetUpCleanTestQueue();
            //TestHelper.AddAMessage();
            queue.SetUpWatchQueue(queue);
            //var pubSubChannel = new PublishSubscribeChannel<T>(queue);
            target.AddSubscriber(typeof(TestSubscriber<T>));
            // List<ISubscriber<T>> Subscribers = pubSubChannel.GetSubscribers();


            target.Start();

            DateTime start = DateTime.Now;
            while (!TestHelper.IsQueueEmpty(queue.Name))
            {

                System.Threading.Thread.Sleep(1000);
                Assert.IsTrue(DateTime.Now > start + new TimeSpan(10000), "I give up spent to much time processing");
            }

            target.Stop();


            log = new LogEntry();
            DateTime Endprocessing = DateTime.Now;
            log.Message = ("Finished processing: tIME:" + Endprocessing);
            Logger.Write(log);
            log.Message = ("Finished processing: QUeuetIME:" + (startTime - Endprocessing).ToString());
            Logger.Write(log);



            log.Message = ("Finished processing: QUeuetIME:" + (startTime - Endprocessing).ToString());
            Logger.Write(log);
            System.Diagnostics.Debug.WriteLine("Finished processing: tIME:" + Endprocessing);
        }

        [TestCategory("Performance"), TestMethod()]
        public void RunPubSubTest()
        {
            RunPubSubTestHelper<Message>();
        }

        public static DateTime startTime;

        public void RunPubSubTestHelper<T>() where T : Message
        {
            Debug.Listeners.Add(new TextWriterTraceListener("log.txt"));
            Debug.AutoFlush = true;

            LogEntry log = new LogEntry();
            startTime = DateTime.Now;
            log.Message = ("Started loading msmq: tIME:" + startTime);
            Logger.Write(log);
            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);

            ///create queue and channel
            var pubsub = new PublishSubscribeChannel<Message>(new MsmqQueueProvider<Message>())
                .AddSubscriber(typeof(BusinessLogic.TestSubscriber<Message>))
                .AddSubscriber(typeof(BusinessLogic.TestSubscriber2<Message>));

            TestHelper.SetUpCleanTestQueue("EntitiesMessage");
           // queue.SetUpWatchQueue(queue);
            //IQueueProvider<T> Queue = new UserQueueProvider() as IQueueProvider<T>;

            Random r = new Random();
            pubsub.Start();
            Parallel.For(0, 5, (j) =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    Message m = GetNewMessage<Message>();

                    pubsub.PutMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(20,30));
                    System.Threading.Thread.Sleep(30);
                }
            });
            log = new LogEntry();
            DateTime startprocessing = DateTime.Now;
            log.Message = ("Started processing: tIME:" + DateTime.Now);
            Logger.Write(log);
            System.Diagnostics.Debug.WriteLine("Started processing: tIME:" + startTime);

            DateTime start = DateTime.Now;
            while (!TestHelper.IsQueueEmpty())
            {
                System.Threading.Thread.Sleep(1000);
                if (DateTime.Now > start + new TimeSpan(0, 20, 0)) break;
            }

            pubsub.Stop();

            log = new LogEntry();
            DateTime Endprocessing = DateTime.Now;
            Debug.WriteLine("Finished processing: tIME:" + Endprocessing);
           
            Debug.WriteLine("Finished processing: QUeuetIME:" + (startTime - Endprocessing).ToString());
            Debug.WriteLine("Finished processing: QUeuetIME IN GECONDS:" + (startTime - Endprocessing).TotalSeconds.ToString());

            System.Diagnostics.Debug.WriteLine("Finished processing: tIME:" + Endprocessing);

            Debug.WriteLine(Counter.TotalSubscriberCount().ToString());
            Debug.WriteLine(" Rate: " + ((startTime - Endprocessing).TotalSeconds / Counter.TotalSubscriberCount()).ToString());


        }

        private static T GetNewMessage<T>() where T : Message
        {
            Message m = new Message();
            m.Name = "Gwen";
            m.BatchNumber = 1;
            m.Guid = System.Guid.NewGuid();
            m.MessageID = "MessageID";
            m.SubscriptionID = "SubscriptionID";
            m.MessagePutTime = DateTime.Now;
            return (T)m;
        }

    }

}
