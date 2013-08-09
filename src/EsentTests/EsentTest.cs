using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using Microsoft.Isam.Esent.Interop;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
//using Phantom.PubSub.Fakes;
using Ninject;

namespace EsentTests
{
    [TestClass]
    public class EsentTest
    {
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            var store = new EsentStoreProvider<Dummy>();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //EsentInstanceService<Dummy>.Service.EsentInstance.Dispose();
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void ConfigureDatabase()
        {
            var store = new EsentStoreProvider<Dummy>();

            TestHelper.VerifyDatabase("EsentTestsDummy");

        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void DoesDatabaseExistReturnsTrue()
        {
            //string binFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string binFolder = Environment.CurrentDirectory;

            //TestHelper.CreateDatabase("EsentTestsDummy");
            IList<string> dllFiles = Directory.GetFiles(binFolder, "*.edb", SearchOption.AllDirectories).ToList();
            Assert.IsTrue(dllFiles.Count > 0);
            if (dllFiles.Count > 0)
            {
                var result = "notfound";
                foreach (var dllFile in dllFiles)
                {
                    if (dllFile.Contains("PhantomPubSub.edb"))
                    {
                        result = "found";
                    }
                }
                Assert.AreEqual("found", result);
            }

            var store = new EsentStoreProvider<Dummy>();

            TestHelper.VerifyDatabase("EsentTestsDummy");
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void DoesStoreExistReturnsFalse()
        {
            TestHelper.CreateDatabase("EsentTestsDummyx"); 
            Assert.IsFalse(EsentConfig.DoesStoreExist<DummyDoesntExist>("Dummy"));
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void PutMessageTest()
        {
            //TestHelper.CreateDatabase("EsentTestsDummy.edb");
            string result = string.Empty;
            var store = new EsentStoreProvider<Dummy>();
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 }));

            Assert.IsTrue(Convert.ToInt32(result) >= 1);
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void PutMessageTest_UsingRepositoryAndUnitofWork()
        {
            if (!TestHelper.DoesDatabaseExist("EsentTestsDummy.edb")) TestHelper.CreateDatabase("EsentTestsDummy.edb");
            string result = string.Empty;
            using (var store = new EsentStore<Dummy>(true))
            {
                Repository<Dummy> repository = new Repository<Dummy>(store);
                var message = TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 });
                //result = repository.AddAMessage(TestHelper.Serialize((Dummy)message.Body), TestHelper.Serialize(message)).ToString();
                result = repository.AddMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 })).ToString();
            }
            Assert.IsTrue(Convert.ToInt32(result) >= 1);
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void GetAllMessagesFromStoreTest()
        {
            //TestHelper.CreateDatabase("EsentTestsDummy.edb");
            var store = new EsentStoreProvider<Dummy>();

            var preresults = store.GetAllMessages();
            int startCount = 0;
            startCount = startCount + preresults.Count;
            var result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName1", Id = 14 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName2", Id = 15 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName3", Id = 16 }));
            List<MessagePacket<Dummy>> results = store.GetAllMessages();
            Assert.AreEqual(4 + startCount, results.Count);
            Assert.IsInstanceOfType(results[0], typeof(MessagePacket<Dummy>));
            Assert.IsInstanceOfType(results[0].Body, typeof(Dummy));
            Assert.IsInstanceOfType(results[1].Body, typeof(Dummy));
            Assert.IsTrue(results[1].Body.Id > 12 && results[1].Body.Id < 17);
            Assert.IsTrue(results[1].Body.Name.StartsWith("THeDummysName"));

        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void GetRecordCountStoreTest()
        {
            //TestHelper.CreateDatabase("EsentTestsDummy.edb");
            var store = new EsentStoreProvider<Dummy>();
            var pubsub = new PublishSubscribeChannel<Dummy>(store);

            var firstCount = pubsub.Count;
            var result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 }));
            Assert.AreEqual(1, pubsub.Count - firstCount);
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 14 }));
            Assert.AreEqual(2, pubsub.Count - firstCount);
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void CheckSerialization()
        {
            var store = new EsentStoreProvider<Dummy>();

            var preresults = store.GetAllMessages();
            int startCount = 0;
            startCount = startCount + preresults.Count;
            var result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 }));

            List<MessagePacket<Dummy>> results = store.GetAllMessages();
            MessagePacket<Dummy> messagePacket = (MessagePacket<Dummy>)results.SingleOrDefault(mp => mp.MessageId == Convert.ToInt32(result));

            Assert.IsInstanceOfType(messagePacket, typeof(MessagePacket<Dummy>));
            Assert.IsInstanceOfType(messagePacket.Body, typeof(Dummy));
            Assert.IsTrue(messagePacket.SubscriberMetadataList != null);
            Assert.IsTrue(messagePacket.SubscriberMetadataList.Count == 2);
            Assert.IsInstanceOfType(messagePacket.SubscriberMetadataList[0], typeof(ISubscriberMetadata));

            Assert.IsInstanceOfType(messagePacket.SubscriberMetadataList[1], typeof(SubscriberMetadata));
            Assert.IsTrue(messagePacket.SubscriberMetadataList[0].Name == "John");
            Assert.IsTrue(messagePacket.SubscriberMetadataList[1].Name == "Joe");
            Assert.IsTrue(messagePacket.Body.Id == 13);
            Assert.IsTrue(messagePacket.Body.Name.StartsWith("THeDummysName"));

        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void GetAllMessagesFromStoreProviderTest()
        {
            //TestHelper.CreateDatabase("EsentTestsDummy.edb");

            var store = new EsentStoreProvider<Dummy>();

            var preresults = store.GetAllMessages();
            int startCount = 0;
            startCount = startCount + preresults.Count;
            var result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName1", Id = 14 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName2", Id = 15 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName3", Id = 16 }));
            List<MessagePacket<Dummy>> results = store.GetAllMessages();
            Assert.AreEqual(4 + startCount, results.Count);
            Assert.IsInstanceOfType(results[0], typeof(MessagePacket<Dummy>));
            Assert.IsInstanceOfType(results[0].Body, typeof(Dummy));
            Assert.IsInstanceOfType(results[1].Body, typeof(Dummy));
            Assert.IsTrue(results[1].Body.Id > 12 && results[1].Body.Id < 17);
            Assert.IsTrue(results[1].Body.Name.StartsWith("THeDummysName"));

        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void RemoveFromStoreTest()
        {
            var store = new EsentStoreProvider<Dummy>();
            var preresults = store.GetAllMessages();
            int startCount = 0;
            startCount = startCount + preresults.Count;

            var result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName1", Id = 14 }));
            var result2 = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName2", Id = 15 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName3", Id = 16 }));
            store.DeleteMessage(result2);
            List<MessagePacket<Dummy>> results = store.GetAllMessages();
            Assert.AreEqual(startCount + 3, results.Count);
            var emptyResults = results.Where(m => m.Id == result2);
            Assert.IsTrue(0 == emptyResults.Count());
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void RunStoreProviderInParallel()
        {
            //Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\log.txt"));
            //Trace.AutoFlush = true;
            var startTime = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);

            Random r = new Random();

            int numbertoinsert = 1000;
            int startRange = 40;
            int endRange = 50;

            var stopwatch = Stopwatch.StartNew();

            var t1 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    TestHelper.Add4MessagesToStore();

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    TestHelper.Add4MessagesToStore();

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    TestHelper.Add4MessagesToStore();

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            Task.WaitAll(t1, t2, t3);

            int totalprocessed = numbertoinsert * 2 * 4;
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            float elapsedseconds = elapsedTime / 1000;

            Trace.WriteLine("");

            Trace.WriteLine("Finished processing: time: " + elapsedseconds + " seconds");
            Trace.WriteLine("Number processed: " + totalprocessed.ToString());
            float rate = totalprocessed / elapsedseconds;
            double timeToProcess = elapsedTime / totalprocessed;
            Trace.WriteLine("Rate per second: " + rate.ToString());
            Trace.WriteLine("time per message: " + timeToProcess.ToString() + " ms");
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void Publish_1_Message_Esent()
        {
            Publish_1_MessageHelper<Customer1>();
        }

        private void Publish_1_MessageHelper<T>() where T : Customer1
        {
            //Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\log.txt"));
            //Trace.AutoFlush = true;

            DateTime start = DateTime.Now;

            var stopwatch = Stopwatch.StartNew();
            var pubsub = new PublishSubscribeChannel<Customer1>(new EsentStoreProvider<Customer1>())
                        .AddSubscriberType(typeof(SpeedySubscriber<Customer1>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                        .AddSubscriberType(typeof(SpeedySubscriber2<Customer1>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            Customer1 m = new Customer1();

            pubsub.PublishMessage((T)m);

            //var t1 = Task<int>.Factory.StartNew(() =>
            //{
            //    int result = 0;

            //    pubsub.PublishMessage((T)m);

            //    return result;
            //});


            System.Threading.Thread.Sleep(1000);

            var t6 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                while (pubsub.Count > 0)
                {

                    System.Threading.Thread.Sleep(1000);
                    Assert.IsTrue(DateTime.Now < start + new TimeSpan(10000), "I give up spent to much time processing");
                }
                return result;
            });

            Task.WaitAll(t6);



            Trace.WriteLine("Counter Index: 0 Total Count: " + Counter.Subscriber(0).ToString() + " Total Subscribers that were started");
            Trace.WriteLine("Counter Index: 1 Total Count: " + Counter.Subscriber(1).ToString() + " Total Subscriber that were started");
        }

        [TestCategory("Performance"), TestMethod()]
        public void RunPubSub_With_esent_Test()
        {
            RunPubSub_With_esent_TestHelper<Customer>();
        }

        public static DateTime startTime;

        public void RunPubSub_With_esent_TestHelper<T>() where T : Customer
        {

            //var fileInfo = new FileInfo(@"D:\\Dev\\temp\\RunPubSub_With_esent_Test.txt");
            //fileInfo.Delete();
            //Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\RunPubSub_With_esent_Test.txt"));
            //Trace.AutoFlush = true;
            startTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);
            Trace.WriteLine("Started loading msmq: tIME: Ticks: " + DateTime.Now.Ticks);
            ///create queue and channel
            var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
                .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
                .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            Random r = new Random();

            int numbertoinsert = 100;
            int startRange = 40;
            int endRange = 50;
            var t1 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "x" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t2 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "y" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    Customer m = new Customer();
                    m.Name = "z" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    Customer m = new Customer();
                    m.Name = "q" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    Customer m = new Customer();
                    m.Name = "p" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });


            var t6 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                int count = pubsub.Count;
                while (count > 0)
                {
                    //var storeprovider = new EsentStoreProvider<Customer>();
                    count = pubsub.Count;
                    Trace.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxCurrent record cxount: " + count.ToString());

                    //var messages =  storeprovider.GetAllMessages();
                    //foreach (var item in messages)
                    //{
                    //    Trace.WriteLine("Name: " + item.Name + "MessageId: " + item.MessageId);
                    //}


                    System.Threading.Thread.Sleep(10000);
                    Assert.IsTrue(DateTime.Now > startTime + new TimeSpan(10000), "I give up spent to much time processing");
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
            Trace.WriteLine(string.Format("Finished processing: tIME: {0} ms", elapsedTime));

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
            Trace.WriteLine(" Time per message: " + timeToProcess.ToString() + " seconds");
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
            Trace.WriteLine("Counter Index: 12 Total Count: " + Counter.Subscriber(12).ToString() + " Number of subscribers that Canceled out");
            Trace.WriteLine("Counter Index: 13 Total Count: " + Counter.Subscriber(13).ToString() + " Number of subscribers that Faulted out");
            Trace.WriteLine("Counter Index: 14 Total Count: " + Counter.Subscriber(14).ToString() + " Number of subscribers that Ran to completion");
            Trace.WriteLine("Counter Index: 15 Total Count: " + Counter.Subscriber(15).ToString() + " Number of times Abort was called");
            Trace.WriteLine("Counter Index: 16 Total Count: " + Counter.Subscriber(16).ToString() + " Number of cancellations recorded");
            Trace.WriteLine("Counter Index: 17 Total Count: " + Counter.Subscriber(17).ToString() + " Number of put in queue");
            Trace.WriteLine("Counter Index: 18 Total Count: " + Counter.Subscriber(18).ToString() + " number of single subscribers/transactions put back in the queue after canceling");
            Trace.WriteLine("Counter Index: 19 Total Count: " + Counter.Subscriber(19).ToString() + " number of times in CreateSingleSubscriberMessagePacket");
            Trace.WriteLine("Counter Index: 20 Total Count: " + Counter.Subscriber(20).ToString() + " number of single subscribers/transactions put back in the queue after exception");
        }


        [TestCategory("Performance"), TestMethod()]
        public void RunPubSub_With_Multiple_esent_Test()
        {
            RunPubSub_With_esent_TestHelper<Customer>();
        }

        public void RunPubSub_With_Multiple_esent_TestHelper<T>() where T : Customer
        {
            //var fileInfo = new FileInfo(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_esent_Testlog.txt");
            //fileInfo.Delete();

            //Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_esent_Testlog.txt"));
            //Trace.AutoFlush = true;
            var start = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);
            Trace.WriteLine("Started loading msmq: tIME: Ticks: " + DateTime.Now.Ticks);
            ///create queue and channel
            var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
                .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
                .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
                .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            var pubsub2 = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            var pubsub3 = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            var pubsub4 = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            var pubsub5 = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            Random r = new Random();

            int numbertoinsert = 100;
            int startRange = 40;
            int endRange = 50;
            var t1 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "x" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t2 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "y" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    Customer m = new Customer();
                    m.Name = "z" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    Customer m = new Customer();
                    m.Name = "q" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
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
                    Customer m = new Customer();
                    m.Name = "p" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });


            var t6 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                while (pubsub.Count > 0)
                {

                    System.Threading.Thread.Sleep(5000);
                    Assert.IsTrue(DateTime.Now > start + new TimeSpan(10000), "I give up spent to much time processing");
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
            Trace.WriteLine(string.Format("Finished processing: tIME: {0} ms", elapsedTime));

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
            Trace.WriteLine(" Time per message: " + timeToProcess.ToString() + " seconds");
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
            Trace.WriteLine("Counter Index: 12 Total Count: " + Counter.Subscriber(12).ToString() + " Number of subscribers that Canceled out");
            Trace.WriteLine("Counter Index: 13 Total Count: " + Counter.Subscriber(13).ToString() + " Number of subscribers that Faulted out");
            Trace.WriteLine("Counter Index: 14 Total Count: " + Counter.Subscriber(14).ToString() + " Number of subscribers that Ran to completion");
            Trace.WriteLine("Counter Index: 15 Total Count: " + Counter.Subscriber(15).ToString() + " Number of times Abort was called");
            Trace.WriteLine("Counter Index: 16 Total Count: " + Counter.Subscriber(16).ToString() + " Number of cancellations recorded");
            Trace.WriteLine("Counter Index: 17 Total Count: " + Counter.Subscriber(17).ToString() + " Number of put in queue");
            Trace.WriteLine("Counter Index: 18 Total Count: " + Counter.Subscriber(18).ToString() + " number of single subscribers/transactions put back in the queue after canceling");
            Trace.WriteLine("Counter Index: 19 Total Count: " + Counter.Subscriber(19).ToString() + " number of times in CreateSingleSubscriberMessagePacket");
            Trace.WriteLine("Counter Index: 20 Total Count: " + Counter.Subscriber(20).ToString() + " number of single subscribers/transactions put back in the queue after exception");
        }

        //[TestCategory("Performance"), TestMethod()]
        //public void RunPubSub_With_AlwaysFailSubscriber_esent_Test()
        //{
        //    RunPubSub_With_AlwaysFailSubscriber_esent_Test_TestHelper<Customer>();
        //}

        //public void RunPubSub_With_AlwaysFailSubscriber_esent_Test_TestHelper<T>() where T : Customer
        //{
        //    //Trace.AutoFlush = true;
        //    Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\log4.txt"));
        //    Trace.AutoFlush = true;
        //    var start = DateTime.Now;
        //    var stopwatch = Stopwatch.StartNew();

        //    System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);
        //    Trace.WriteLine("Started loading msmq: tIME: Ticks: " + DateTime.Now.Ticks);
        //    ///create queue and channel
        //    var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
        //        .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
        //        .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
        //        .AddSubscriberType(typeof(TestUtils.SpeedySubscriberGuaranteedExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));
        //    var m = new Customer
        //    {
        //        Name = "John"
        //    };

        //    pubsub.PublishMessage((T)m);

        //    var t6 = Task<int>.Factory.StartNew(() =>
        //    {
        //       // var pubsub2 = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
        //       //.AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
        //       //.AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
        //       //.AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

        //        int result = 0;
        //        while (pubsub.Count > 0)
        //        {

        //            System.Threading.Thread.Sleep(5000);
        //            Assert.IsTrue(DateTime.Now < start + new TimeSpan(10000), "I give up spent to much time processing");
        //        }
        //        return result;
        //    });

        //    t6.Wait();

        //}


        [TestCategory("Performance"), TestMethod()]
        public void RunPubSub_With_Multiple_on_different_Threads_esent_Test()
        {
            RunPubSub_With_Multiple_esent_on_different_Threads_TestHelper<Customer>();
        }

        public void RunPubSub_With_Multiple_esent_on_different_Threads_TestHelper<T>() where T : Customer
        {
            //var fileInfo = new FileInfo(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_on_different_Threads_esent_Testlog.txt");
            //fileInfo.Delete();
            //Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_on_different_Threads_esent_Testlog.txt"));
            //Trace.AutoFlush = true;
            var start = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);
            Trace.WriteLine("Started loading msmq: tIME: Ticks: " + DateTime.Now.Ticks);
            ///create queue and channel



            Random r = new Random();

            int numbertoinsert = 100;
            int startRange = 40;
            int endRange = 50;
            var t1 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "x" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t2 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "y" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t3 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "z" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t4 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "q" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t5 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Customer m = new Customer();
                    m.Name = "p" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });


            var t6 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>())
               .AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100))
               .AddSubscriberType(typeof(SpeedySubscriberRandomExceptions<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                while (pubsub.Count > 0)
                {

                    System.Threading.Thread.Sleep(5000);
                    Assert.IsTrue(DateTime.Now > start + new TimeSpan(10000), "I give up spent to much time processing");
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
            Trace.WriteLine(string.Format("Finished processing: tIME: {0} ms", elapsedTime));

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
            Trace.WriteLine(" Time per message: " + timeToProcess.ToString() + " seconds");
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
            Trace.WriteLine("Counter Index: 12 Total Count: " + Counter.Subscriber(12).ToString() + " Number of subscribers that Canceled out");
            Trace.WriteLine("Counter Index: 13 Total Count: " + Counter.Subscriber(13).ToString() + " Number of subscribers that Faulted out");
            Trace.WriteLine("Counter Index: 14 Total Count: " + Counter.Subscriber(14).ToString() + " Number of subscribers that Ran to completion");
            Trace.WriteLine("Counter Index: 15 Total Count: " + Counter.Subscriber(15).ToString() + " Number of times Abort was called");
            Trace.WriteLine("Counter Index: 16 Total Count: " + Counter.Subscriber(16).ToString() + " Number of cancellations recorded");
            Trace.WriteLine("Counter Index: 17 Total Count: " + Counter.Subscriber(17).ToString() + " Number of put in queue");
            Trace.WriteLine("Counter Index: 18 Total Count: " + Counter.Subscriber(18).ToString() + " number of single subscribers/transactions put back in the queue after canceling");
            Trace.WriteLine("Counter Index: 19 Total Count: " + Counter.Subscriber(19).ToString() + " number of times in CreateSingleSubscriberMessagePacket");
            Trace.WriteLine("Counter Index: 20 Total Count: " + Counter.Subscriber(20).ToString() + " number of single subscribers/transactions put back in the queue after exception");
        }

        [TestCategory("Performance"), TestMethod()]
        public void RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Test()
        {
            RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_TestHelper<CustomerX>();
        }

        public void RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_TestHelper<T>() where T : CustomerX
        {
            //var fileInfo = new FileInfo(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Testlog.txt");
            //fileInfo.Delete();
            //Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Testlog.txt"));
            //Trace.AutoFlush = true;
            var start = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);
            Trace.WriteLine("Started loading msmq: tIME: Ticks: " + DateTime.Now.Ticks);

            Random r = new Random();

            int numbertoinsert = 100;
            int startRange = 40;
            int endRange = 50;
            var t1 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<CustomerX>(new EsentStoreProvider<CustomerX>())
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerX m = new CustomerX();
                    m.Name = "x" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t2 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<CustomerX>(new EsentStoreProvider<CustomerX>())
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerX m = new CustomerX();
                    m.Name = "y" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t3 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<CustomerX>(new EsentStoreProvider<CustomerX>())
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerX m = new CustomerX();
                    m.Name = "z" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t4 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<CustomerX>(new EsentStoreProvider<CustomerX>())
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerX m = new CustomerX();
                    m.Name = "q" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t5 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<CustomerX>(new EsentStoreProvider<CustomerX>())
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 0, 100));
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerX m = new CustomerX();
                    m.Name = "p" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });


            var t6 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = new PublishSubscribeChannel<CustomerX>(new EsentStoreProvider<CustomerX>())
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerX>)).WithTimeToExpire(new TimeSpan(0, 1, 0));

                int result = 0;
                while (pubsub.Count > 0)
                {

                    System.Threading.Thread.Sleep(5000);
                    Assert.IsTrue(DateTime.Now < start + new TimeSpan(0, 10, 0), "I give up spent to much time processing");
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
            Trace.WriteLine(string.Format("Finished processing: tIME: {0} ms", elapsedTime));

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
            Trace.WriteLine(" Time per message: " + timeToProcess.ToString() + " seconds");
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
            Trace.WriteLine("Counter Index: 12 Total Count: " + Counter.Subscriber(12).ToString() + " Number of subscribers that Canceled out");
            Trace.WriteLine("Counter Index: 13 Total Count: " + Counter.Subscriber(13).ToString() + " Number of subscribers that Faulted out");
            Trace.WriteLine("Counter Index: 14 Total Count: " + Counter.Subscriber(14).ToString() + " Number of subscribers that Ran to completion");
            Trace.WriteLine("Counter Index: 15 Total Count: " + Counter.Subscriber(15).ToString() + " Number of times Abort was called");
            Trace.WriteLine("Counter Index: 16 Total Count: " + Counter.Subscriber(16).ToString() + " Number of cancellations recorded");
            Trace.WriteLine("Counter Index: 17 Total Count: " + Counter.Subscriber(17).ToString() + " Number of put in queue");
            Trace.WriteLine("Counter Index: 18 Total Count: " + Counter.Subscriber(18).ToString() + " number of single subscribers/transactions put back in the queue after canceling");
            Trace.WriteLine("Counter Index: 19 Total Count: " + Counter.Subscriber(19).ToString() + " number of times in CreateSingleSubscriberMessagePacket");
            Trace.WriteLine("Counter Index: 20 Total Count: " + Counter.Subscriber(20).ToString() + " number of single subscribers/transactions put back in the queue after exception");
        }

        [TestCategory("Performance"), TestMethod()]
        public void RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Using_NinjectContainer_Test()
        {
            RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Using_NinjectContainer_TestHelper<CustomerY>();
        }

        public void RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Using_NinjectContainer_TestHelper<T>() where T : CustomerY
        {
            //var fileInfo = new FileInfo(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Using_NinjectContainer_TestHelper.txt");
            //fileInfo.Delete();
            //Trace.Listeners.Add(new TextWriterTraceListener(@"D:\\Dev\\temp\\RunPubSub_With_Multiple_esent_on_different_Threads_Speedy_Using_NinjectContainer_TestHelper.txt"));
            //Trace.AutoFlush = true;
            var start = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);
            Trace.WriteLine("Started loading msmq: tIME: Ticks: " + DateTime.Now.Ticks);

            IKernel kernel = new StandardKernel();

            kernel.Bind<Store<CustomerY>>().ToSelf().InSingletonScope();
            kernel.Bind<IStoreProvider<CustomerY>>().To<EsentStoreProvider<CustomerY>>();
            kernel.Bind<IEsentStore<CustomerY>>().To<EsentStore<CustomerY>>();
            kernel.Bind<IPublishSubscribeChannel<CustomerY>>().To<PublishSubscribeChannel<CustomerY>>();

            //need to get a store for duration of application
            var store = kernel.Get<Store<CustomerY>>();

            Random r = new Random();

            int numbertoinsert = 100;
            int startRange = 40;
            int endRange = 50;
            var t1 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = kernel.Get<IPublishSubscribeChannel<CustomerY>>()
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerY m = new CustomerY();
                    m.Name = "x" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t2 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = kernel.Get<IPublishSubscribeChannel<CustomerY>>()
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerY m = new CustomerY();
                    m.Name = "y" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });

            var t3 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = kernel.Get<IPublishSubscribeChannel<CustomerY>>()
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerY m = new CustomerY();
                    m.Name = "z" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t4 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = kernel.Get<IPublishSubscribeChannel<CustomerY>>()
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerY m = new CustomerY();
                    m.Name = "q" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    result = i;
                }
                return result;
            });

            var t5 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = kernel.Get<IPublishSubscribeChannel<CustomerY>>()
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(SpeedySubscriber2<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 0, 100));
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    CustomerY m = new CustomerY();
                    m.Name = "p" + i.ToString();
                    pubsub.PublishMessage((T)m);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });


            var t6 = Task<int>.Factory.StartNew(() =>
            {
                var pubsub = kernel.Get<IPublishSubscribeChannel<CustomerY>>()
               .AddSubscriberType(typeof(SpeedySubscriber<CustomerY>)).WithTimeToExpire(new TimeSpan(0, 1, 0));

                int result = 0;
                while (pubsub.Count > 0)
                {

                    System.Threading.Thread.Sleep(5000);
                    Assert.IsTrue(DateTime.Now < start + new TimeSpan(0, 10, 0), "I give up spent to much time processing");
                }
                return result;
            });

            Task.WaitAll(t1, t2, t3, t4, t5, t6);
            store.Dispose();

            Trace.WriteLine("Started processing: tIME:" + startTime);
            Trace.WriteLine("Finished in the loop: " + t6.Result.ToString());
            int totalprocessed = t1.Result + t2.Result + t3.Result + t4.Result + t5.Result + 5;
            Trace.WriteLine("Total processed = :" + (t1.Result + t2.Result + t3.Result + t4.Result + t5.Result + 5).ToString());

            var elapsedTime = stopwatch.ElapsedMilliseconds;
            float elapsedseconds = elapsedTime / 1000;
            DateTime Endprocessing = DateTime.Now;

            Trace.WriteLine("");
            Trace.WriteLine("Finished processing: tIME: " + Endprocessing);
            Trace.WriteLine(string.Format("Finished processing: tIME: {0} ms", elapsedTime));

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
            Trace.WriteLine(" Time per message: " + timeToProcess.ToString() + " seconds");
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
            Trace.WriteLine("Counter Index: 12 Total Count: " + Counter.Subscriber(12).ToString() + " Number of subscribers that Canceled out");
            Trace.WriteLine("Counter Index: 13 Total Count: " + Counter.Subscriber(13).ToString() + " Number of subscribers that Faulted out");
            Trace.WriteLine("Counter Index: 14 Total Count: " + Counter.Subscriber(14).ToString() + " Number of subscribers that Ran to completion");
            Trace.WriteLine("Counter Index: 15 Total Count: " + Counter.Subscriber(15).ToString() + " Number of times Abort was called");
            Trace.WriteLine("Counter Index: 16 Total Count: " + Counter.Subscriber(16).ToString() + " Number of cancellations recorded");
            Trace.WriteLine("Counter Index: 17 Total Count: " + Counter.Subscriber(17).ToString() + " Number of put in queue");
            Trace.WriteLine("Counter Index: 18 Total Count: " + Counter.Subscriber(18).ToString() + " number of single subscribers/transactions put back in the queue after canceling");
            Trace.WriteLine("Counter Index: 19 Total Count: " + Counter.Subscriber(19).ToString() + " number of times in CreateSingleSubscriberMessagePacket");
            Trace.WriteLine("Counter Index: 20 Total Count: " + Counter.Subscriber(20).ToString() + " number of single subscribers/transactions put back in the queue after exception");
        }
    }
}
