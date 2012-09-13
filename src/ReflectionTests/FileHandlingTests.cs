using System;
using System.Reflection;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using System.Diagnostics;
using System.IO;
using System.Messaging;
using System.Threading.Tasks;

namespace ReflectionTests
{
    [TestClass]
    public class FileHandlingTests
    {
        [TestMethod]
        public void HowToUsePubSubChannelwithSubscribersJustDroppedinBin()
        {
            var pubSubChannel = new PublishSubscribeChannel<Dummy>(new MsmqStoreProvider<Dummy>());
            pubSubChannel.PublishMessage(new Dummy() { Name = "Johnny"});  
        }

        [TestMethod]
        public void GetSubscriberInfo_For_The_Type_DummyWithDefaultTimeToExpire()
        {
            var result = GetSubscriberInfosHelper<Dummy>("DummySubscriberWithDefault");
            Assert.AreEqual(result.Item1, "DummySubscriberWithDefault");
            Assert.AreEqual(result.Item2.Name, typeof(DummySubscriberWithDefault).Name);
            Assert.AreEqual(20, result.Item3.TotalSeconds);
            //Assert.AreEqual(result.Item2, typeof(BusinessLogic.TestSubscriberXXX<Entities.Message>));
        }

        [TestMethod]
        public void GetSubscriberInfo_For_The_Type_DummyWithOutDefaultTimeToExpire()
        {
            var result = GetSubscriberInfosHelper<Dummy>("DummySubscriberWithOutDefault");
            Assert.AreEqual(result.Item1, "DummySubscriberWithOutDefault");
            Assert.AreEqual(result.Item2.Name, typeof(DummySubscriberWithOutDefault).Name);
            Assert.AreEqual(100, result.Item3.TotalSeconds);
            //Assert.AreEqual(result.Item2, typeof(BusinessLogic.TestSubscriberXXX<Entities.Message>));
        }

        public Tuple<string, Type, TimeSpan> GetSubscriberInfosHelper<T>(string NameofSubscriberLookingFor)
        {
            Tuple<string, Type, TimeSpan> returnValue = null;
            var result = AutoConfig<T>.SubscriberInfos;
            foreach (var item in result)
            {
                Assert.IsInstanceOfType(item, typeof(Tuple<string, Type, TimeSpan>));
                if (NameofSubscriberLookingFor == item.Item1)
                {
                    returnValue = item;
                }
            }
            return returnValue;
        }


        [TestMethod]
        public void Publish_1_Message_AutoConfig()
        {
            Publish_1_MessageHelper<Dummy>();
        }

        private void Publish_1_MessageHelper<T>() where T : Dummy
        {
            Trace.Listeners.Add(new TextWriterTraceListener(@"C:\\Dev\\temp\\log.txt"));
            Trace.AutoFlush = true;
            var startTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();
            System.Diagnostics.Debug.WriteLine("Started loading msmq: tIME:" + startTime);
            Trace.WriteLine("Started loading msmq: tIME: Ticks: " + DateTime.Now.Ticks);

            var pubsub = new PublishSubscribeChannel<Dummy>(new MsmqStoreProvider<Dummy>());

            SetUpCleanTestQueue("ReflectionTestsDummy");
            

            int numbertoinsert = 1000;
            int startRange = 40;
            int endRange = 50;
            Random r = new Random();

            var t1 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                for (int i = 0; i < numbertoinsert; i++)
                {
                    Dummy d = GetNewMessage<Dummy>();

                    pubsub.PublishMessage((T)d);

                    //System.Threading.Thread.Sleep(r.Next(startRange, endRange));
                    //System.Threading.Thread.Sleep(30);
                    result = i;
                }
                return result;
            });


            var t2 = Task<int>.Factory.StartNew(() =>
            {
                int result = 0;
                while (!IsQueueEmpty("ReflectionTestsDummy"))
                {

                    System.Threading.Thread.Sleep(1000);
                }
                return result;
            });

            Task.WaitAll(t1, t2);

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


        [TestMethod]
        public void TestMethod1()
        {
            Uri uri = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
            
           var d = Environment.CurrentDirectory;
//            //var directory = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = new DirectoryInfo(uri.LocalPath);
            var dlls = di.GetFiles("*.dll");
           
            Assembly[] appAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in dlls)
            {
                var ass = System.Reflection.Assembly.ReflectionOnlyLoadFrom(item.FullName);

            }
//            foreach (Assembly assembly in appAssemblies)
//            {
//                Debug.WriteLine(assembly.FullName);
//            }
            
////For Each asm As System.Reflection.Assembly In
//// System.AppDomain.CurrentDomain.GetAssemblies()
//// For Each mdl As System.Reflection.Module In asm.GetModules()
//// For Each typ As System.Type In mdl.GetTypes()
//// If typ.IsSubclassOf(GetType(MyBasePage))
 
//            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()
//                            where t.GetInterfaces().Contains(typeof(IFindMe))
//                            && t.GetConstructor(Type.EmptyTypes) != null
//                            select t;
//            foreach (var item in instances)
//            {
//                Assert.AreEqual(typeof(BusinessLogic.TestMessageSubscriber2), item);
                
//            }
        }

        public static void SetUpCleanTestQueue(string QueueName)
        {
            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\" + QueueName))
                MessageQueue.Delete(@".\private$\" + QueueName);
            MessageQueue.Create(@".\private$\" + QueueName);

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\" + QueueName + "PoisonMessages"))
                MessageQueue.Delete(@".\private$\" + QueueName + "PoisonMessages");
            MessageQueue.Create(@".\private$\" + QueueName + "PoisonMessages");
        }

        private static T GetNewMessage<T>() where T : Dummy
        {
            Dummy m = new Dummy();
            m.Name = "Gwen";
            return (T)m;
        }

        public static bool IsQueueEmpty(string QueueName)
        {
            var msgQ = new MessageQueue(@".\private$\" + QueueName);
            

            bool isQueueEmpty = false;
            try
            {
                msgQ.Peek(new TimeSpan(0));
                //}
                isQueueEmpty = false;
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    isQueueEmpty = true;
                }
            }
            return isQueueEmpty;
        } 
    }
}
