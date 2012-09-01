using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BusinessLogic;
using Entities;
using System.Collections.Generic;
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

        [TestMethod]
        public void CreateMSMQProvider()
        {
            var queue = new MsmqQueueProvider<Message>();
            Assert.IsInstanceOfType(queue, typeof(MsmqQueueProvider<Message>));
            Assert.AreEqual("EntitiesMessage", queue.Name);
        }


        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ProcessBatch_1_MessageTest()
        {
            TestHelper.SetUpCleanTestQueue("EntitiesMessage");
            var MessagePubSubChannel = new PublishSubscribeChannel<Message>(new MsmqQueueProvider<Message>());          
            MessagePubSubChannel.AddSubscriberType(typeof(BusinessLogic.TestSubscriber<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 0, 0, 1));
            TestHelper.AddAMessageMessageWith1MillisecondTTE();
            
            MessagePubSubChannel.ProcessBatch();

            Assert.IsTrue(TestHelper.IsQueueEmpty("EntitiesMessage"));
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ProcessBatch_2_MessageTest()
        {
            TestHelper.SetUpCleanTestQueue("EntitiesMessage");
            var MessagePubSubChannel = new PublishSubscribeChannel<Message>(new MsmqQueueProvider<Message>());
            MessagePubSubChannel.AddSubscriberType(typeof(BusinessLogic.TestSubscriber<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 0, 0, 1))
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriber2<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 0, 0, 1));

            TestHelper.AddAMessageMessageWith1MillisecondTTE();
            TestHelper.AddAMessageMessageWith1MillisecondTTE();

            MessagePubSubChannel.ProcessBatch();

            Assert.IsTrue(TestHelper.IsQueueEmpty("EntitiesMessage"));
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

            queue.SetupWatchQueue(queue);

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
            var PublishSubscribeChannel = CreatePublishSubscribeChannel();
            PublishSubscribeChannel.AddSubscriberType(typeof(BusinessLogic.TestSubscriber<User>)).WithTimeToExpire(new TimeSpan(0, 0, 0, 0, 1));
            
            PublishSubscribeChannel.ProcessBatch();
            Assert.IsTrue(TestHelper.IsQueueEmpty("EntitiesUser"));
        }

        internal virtual IPublishSubscribeChannel<User> CreatePublishSubscribeChannel()
        {
            var queue = CreateQueueProvider();
            TestHelper.SetUpCleanTestQueue();
            TestHelper.AddAMessage("EntitiesUser");
            queue.SetupWatchQueue(queue);
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
            TestHelper.SetUpCleanTestQueue("EntitiesMessage");
            var target = new PublishSubscribeChannel<Message>(new MsmqQueueProvider<Message>())
               .AddSubscriberType(typeof(BusinessLogic.TestSubscriber<Message>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(BusinessLogic.TestSubscriber2<Message>)).WithTimeToExpire(new TimeSpan(0, 1, 0));

            Trace.WriteLine("Started loading msmq: tIME:" + DateTime.Now);

            for (int i = 0; i < 2000; i++)
            {
                TestHelper.AddAMessageMessage();
            }
            Trace.WriteLine("Started processing: tIME:" + DateTime.Now);
           // List<ISubscriber<T>> Subscribers = GetSubscribers<T>();
            BatchProcessor<Message>.ConfigureWithPubSubChannel(target);

            //for (int i = 0; i < 1000; i++)
            //{
            //    TestHelper.AddAMessageMessage();
            //}

            DateTime start = DateTime.Now;
            while (!TestHelper.IsQueueEmpty("EntitiesMessage"))
            {
                System.Threading.Thread.Sleep(1000);
                Assert.IsTrue(DateTime.Now > start + new TimeSpan(10000), "I give up spent to much time processing");
            }
            BatchProcessor<Message>.Halt();

            Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub.Tests", "Phantom.PubSub.Tests")).TraceInfo, "Finished processing: tIME:" + DateTime.Now);
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

            IPublishSubscribeChannel<Message> target = TestHelper.CreatePublishSubscribeChannel<Message>(); // TODO: Initialize to an appropriate value

            Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub.Tests", "Phantom.PubSub.Tests")).TraceInfo, "Started processing: tIME: " + DateTime.Now);

            for (int i = 0; i < 5000; i++)
            {
                TestHelper.AddAMessageMessage();
            }

            Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub.Tests", "Phantom.PubSub.Tests")).TraceInfo, "Started loading msmq: tIME: " + DateTime.Now);

            target.AddSubscriberType(typeof(TestSubscriber<T>)).WithTimeToExpire(new TimeSpan(0,1,0));
            
            BatchProcessor<Message>.ConfigureWithPubSubChannel(target);
            Assert.IsTrue(BatchProcessor<Message>.IsConfigured);
            Assert.IsTrue(BatchProcessor<Message>.HasStarted);

            DateTime start = DateTime.Now;
            while (!TestHelper.IsQueueEmpty("EntitiesMessage"))
            {
                System.Threading.Thread.Sleep(1000);
                Assert.IsTrue(DateTime.Now > start + new TimeSpan(10000), "I give up spent to much time processing");
            }


            DateTime Endprocessing = DateTime.Now;
            Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub.Tests", "Phantom.PubSub.Tests")).TraceInfo, "Finished processing: tIME: " + DateTime.Now);
            Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub.Tests", "Phantom.PubSub.Tests")).TraceInfo, "Finished processing: QUeuetIME:" + (startTime - Endprocessing).ToString());
            Trace.WriteLineIf((new TraceSwitch("Phantom.PubSub.Tests", "Phantom.PubSub.Tests")).TraceInfo, "Finished processing: tIME:" + Endprocessing);
        }

        [TestCategory("Performance"), TestMethod()]
        public void RunPubSubTest()
        {
            RunPubSubTestHelper<Message>();
        }

        public static DateTime startTime;

        public void RunPubSubTestHelper<T>() where T : Message
        {
            //Debug.Listeners.Add(new TextWriterTraceListener("log.txt"));
            //Debug.AutoFlush = true;
            startTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();
            
            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);

            ///create queue and channel
            var pubsub = new PublishSubscribeChannel<Message>(new MsmqQueueProvider<Message>())
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriber<Message>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriberXXX<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 30))
                .AddSubscriberType(typeof(BusinessLogic.TestSubscriber2<Message>)).WithTimeToExpire(new TimeSpan(0, 0, 30));

            TestHelper.SetUpCleanTestQueue("EntitiesMessage");
           // queue.SetUpWatchQueue(queue);
            //IQueueProvider<T> Queue = new UserQueueProvider() as IQueueProvider<T>;

            Random r = new Random();
           // BatchProcessor<Message>.ConfigureWithPubSubChannel(pubsub);
            //Parallel.For(0, 5, (j) =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Message m = GetNewMessage<Message>();

            //        pubsub.PublishMessage((T)m);

            //        System.Threading.Thread.Sleep(r.Next(10,20));
            //        //System.Threading.Thread.Sleep(30);
            //    }
            //});
            int numbertoinsert = 1000;
            var t1 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Message m = GetNewMessage<Message>();

                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(10, 20));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });
            
            var t2 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Message m = GetNewMessage<Message>();

                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(10, 20));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t3 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Message m = GetNewMessage<Message>();

                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(10, 20));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t4 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Message m = GetNewMessage<Message>();

                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(10, 20));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t5 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Message m = GetNewMessage<Message>();

                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(10, 20));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });
           

            var t6 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                while (!TestHelper.IsQueueEmpty("entitiesmessage"))
                {

                    System.Threading.Thread.Sleep(1000);
                    //Assert.IsTrue(DateTime.Now > start + new TimeSpan(10000), "I give up spent to much time processing");
                }
                return result;
            });

            Task.WaitAll(t1, t2, t3, t4, t5, t6);


           Trace.WriteLine("Started processing: tIME:" + startTime);
           Trace.WriteLine("Finished in the loop: " + t6.Result.ToString());
            int totalprocessed = t1.Result + t2.Result + t3.Result + t4.Result + t5.Result + 5;
            Trace.WriteLine("Total processed = :" + (t1.Result + t2.Result + t3.Result + t4.Result + t5.Result + 5).ToString());

            var elapsedTime = stopwatch.ElapsedMilliseconds;
            float elapsedseconds = elapsedTime / 1000;
            DateTime Endprocessing = DateTime.Now;
            
            Trace.WriteLine("");
            Trace.WriteLine("Finished processing: tIME: " + Endprocessing);
            Trace.WriteLine(string.Format("Finished processing: tIME: {0} seconds", elapsedTime));

            Trace.WriteLine("Finished processing: QUeuetIME seconds: " + elapsedseconds);
            float rate = totalprocessed / elapsedseconds;
            float timeToProcess = elapsedseconds / totalprocessed;
            Trace.WriteLine("Rate per second: " + rate.ToString());
           

            Trace.WriteLine("Finished processing: QUeuetIME IN SECONDS: " + (Endprocessing - startTime).TotalSeconds.ToString());

            Trace.WriteLine("Finished processing: tIME:" + Endprocessing);

            Trace.WriteLine("Total count: " + Counter.TotalSubscriberCount().ToString());
            Trace.WriteLine("");
            Trace.WriteLine(" Time per subscription: " + ((Endprocessing - startTime).TotalSeconds / Counter.TotalSubscriberCount()).ToString());
            Trace.WriteLine(" Time per message: " + ((Endprocessing - startTime).TotalSeconds / totalprocessed).ToString());
            Trace.WriteLine(" Time per message: " + timeToProcess.ToString() );
            Trace.WriteLine("");


            Trace.WriteLine("Counter Index: 0 Total Count: " + Counter.Subscriber(0).ToString() + " Total Subscribers that were started");
            Trace.WriteLine("Counter Index: 1 Total Count: " + Counter.Subscriber(1).ToString() + " Total Subscriber that were started");
            Trace.WriteLine("Counter Index: 2 Total Count: " + Counter.Subscriber(2).ToString() + " Total SubscriberCCC that were started");
            Trace.WriteLine("Counter Index: 3 Total Count: " + Counter.Subscriber(3).ToString() + " Total Subscriber2 that were started");
            Trace.WriteLine("Counter Index: 4 Total Count: " + Counter.Subscriber(4).ToString() + " Number of times we peeked in queue");
            Trace.WriteLine("Counter Index: 5 Total Count: " + Counter.Subscriber(5).ToString() + " Number of messages sttempted to Remove from Queues ");
            Trace.WriteLine("Counter Index: 6 Total Count: " + Counter.Subscriber(6).ToString() + " Errors in ProcessAsaBatch reading the queue");
            Trace.WriteLine("Counter Index: 7 Total Count: " + Counter.Subscriber(7).ToString() + " Number of times the Batch processing event was fired");
            Trace.WriteLine("Counter Index: 8 Total Count: " + Counter.Subscriber(8).ToString() + " Number of times the Batch processing event was run because it was not currently running");
            Trace.WriteLine("Counter Index: 9 Total Count: " + Counter.Subscriber(9).ToString() + " Errors in Removing from a nmessage the queue");
            Trace.WriteLine("Counter Index: 10 Total Count: " + Counter.Subscriber(10).ToString() + " Peeks to check if message is still in queue");
            Trace.WriteLine("Counter Index: 11 Total Count: " + Counter.Subscriber(11).ToString() + " errors in Peeks to check if message is still in queue");
             
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
