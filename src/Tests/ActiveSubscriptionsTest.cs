using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Entities;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Phantom.PubSub;
using BusinessLogic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for ActiveSubscriptionsTest and is intended
    ///to contain all ActiveSubscriptionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ActiveSubscriptionsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
        ///A test for Reprocess
        ///</summary>
        public void ReprocessTestParallelHelper<T>()
        {
            ActiveSubscriptions<T> target = new ActiveSubscriptions<T>();
            //AddTest a bunch of active subscriptions
            int i = 0;
            for (int j = 0; j < 1; j++)
            {
                foreach (ISubscriber<T> item in TestHelper.GetSubscribers<T>())
                {
                    item.Id = " SubScription: " + item.Name + ":: MessageID: " + i + "::";
                    target.AddActiveSubscription(item);
                    i++;
                }
            }

            
            
            //get our subscribers
            List<ISubscriber<T>> Subscribers = TestHelper.GetSubscribers<T>();
            //
            List<IMessageStatus<T>> trackIfStarted = TestHelper.GetSubscriptionsStatuses<T>();

            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;

            ISubscriber<T> ActiveSubscription = new TestSubscriber<T>() as ISubscriber<T>;
            ActiveSubscription.Id = "XXXXXXXXX";
            string SubscriptionId = "XXXXXXXXX";
            T message = default(T);
            string MessageId = "YYY";
            ActiveSubscription.TimeToExpire = new TimeSpan(10000);
            ActiveSubscription.OnProcessStartedEvent += new OnProcessStartedEvent(sub_OnProcessStartedEvent);
            ActiveSubscription.OnProcessCompletedEvent += new OnProcessCompletedEvent(sub_OnProcessCompletedEvent);

            actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0],trackIfStarted);
            Assert.AreEqual(actual, false, "should return false because the there are none in the ActiveSubscriptions collection");

            //lets add it calling reprocess should now ensure that it does get reprocessed
            target.AddActiveSubscription(ActiveSubscription);

            Parallel.ForEach<ISubscriber<T>>(Subscribers, (ISubscriber<T> subscriber) =>
            {
                //create our List of ActiveSubscriptions for this specific message
                List<IMessageStatus<T>> trackIfStarted2 = TestHelper.GetSubscribersStatuses<T>();
                actual = target.Reprocess(SubscriptionId, message, MessageId, subscriber.MessageStatusTracker,trackIfStarted2);
                //subscriber.TimeToExpire = new TimeSpan(10000);

                expected = CanProcess<T>(subscriber, subscriber.StartTime + subscriber.TimeToExpire);
                Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");
                //Assert.AreEqual(expected2, actual, "should return true because it is one in the list of ActiveSubscriptions");
            });
        }

        private bool CanProcess<T>(ISubscriber<T> subscriber, DateTime nextstart)
        {
            if (subscriber.Aborted)
            {
                //see if it is time for a restart.
                double time = Math.Pow(2, 1);
                TimeSpan ts = new TimeSpan(0, Convert.ToInt32(time), 0);
                if (DateTime.Compare(DateTime.Now, nextstart) < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void ReprocessParallelTest()
        {
            ReprocessTestParallelHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Reprocess
        ///</summary>
        public void ReprocessTestHelper<T>()
        {
            ActiveSubscriptions<T> target = new ActiveSubscriptions<T>(); 
            string SubscriptionId = "XXXXXXXXX";
            T message = default(T); 
            string MessageId = "YYY";
            List<IMessageStatus<T>> trackIfStarted = TestHelper.GetSubscribersStatuses<T>(); 
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            
            ISubscriber<T> ActiveSubscription = new TestSubscriber<T>() as ISubscriber<T>;
            ISubscriber<T> newActiveSubscription = ActiveSubscription;

            actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0],trackIfStarted);
            Assert.AreEqual(expected, actual, "should return false because the there are none in the ActiveSubscriptions collection");

            ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
            sub.Name = "jackie";
            sub.Id = "XXXXXXXXX";
            sub.TimeToExpire = new TimeSpan(10000);
            sub.OnProcessStartedEvent += new OnProcessStartedEvent(sub_OnProcessStartedEvent);
            sub.OnProcessCompletedEvent += new OnProcessCompletedEvent(sub_OnProcessCompletedEvent);
            target.AddActiveSubscription(sub); 
            actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
            expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
            Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

            //AddTest a bunch more
            foreach (ISubscriber<T> item in TestHelper.GetSubscribers<T>())
            {
                target.AddActiveSubscription(item);
            }

            ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
            sub2.Name = "jackie";
            sub2.Id = "XXXXXXXXX";
            sub2.TimeToExpire = new TimeSpan(10000);
            sub2.OnProcessStartedEvent += new OnProcessStartedEvent(sub_OnProcessStartedEvent);
            sub2.OnProcessCompletedEvent += new OnProcessCompletedEvent(sub_OnProcessCompletedEvent);
            target.AddActiveSubscription(sub);
            actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
            expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
            Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

            actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
            expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
            Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

        }

        void sub_OnProcessCompletedEvent(object sender, ProcessCompletededEventArgs e)
        {

        }

        void sub_OnProcessStartedEvent(object sender, ProcessStartedEventArgs e)
        {
            var current = e.CurrentSubscription as ISubscriber<GenericParameterHelper>;
            current.MessageStatusTracker.StartedProcessing = true;
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void ReprocessTest()
        {
            ReprocessTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        public void AddTestParallelHelper<T>()
        {
            List<ISubscriber<T>> subscribers = TestHelper.GetSubscribers<T>();
            ActiveSubscriptions<T> target = new ActiveSubscriptions<T>();
            Parallel.ForEach<ISubscriber<T>>(subscribers, (ISubscriber<T> subscriber) =>
            {
                target.AddActiveSubscription(subscriber);
            });

            Assert.IsTrue(target.Count == 3, " did not return the expected number");
           // Assert.AreEqual(expected, actual, "Method should return the same object");
        }

        public ActiveSubscriptions<T> AddTestParallelHelper<T>( ActiveSubscriptions<T> SubscriptionList)
        {
            List<ISubscriber<T>> subscribers = TestHelper.GetSubscribers<T>();
            ActiveSubscriptions<T> target = SubscriptionList;
            int Before = target.Count;
            Parallel.ForEach<ISubscriber<T>>(subscribers, (ISubscriber<T> subscriber) =>
            {
                target.AddActiveSubscription(subscriber);
            });

            Assert.IsTrue(target.Count == 3 + Before, " did not return the expected number");
            // Assert.AreEqual(expected, actual, "Method should return the same object");
            return target;
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void AddTestParallelHelper()
        {
            AddTestParallelHelper<GenericParameterHelper>();
            AddTestParallelHelper<User>();
        }
        /// <summary>
        ///A test for Add
        ///</summary>
        public void AddTestHelper<T>()
        {
            ActiveSubscriptions<T> target = new ActiveSubscriptions<T>();
            ISubscriber<T> ActiveSubscription = new TestSubscriber<T>() as ISubscriber<T>;
            ActiveSubscription.Id = "ID";
            ISubscriber<T> expected = ActiveSubscription;
            ISubscriber<T> actual;
            actual = target.AddActiveSubscription(ActiveSubscription);
            Assert.AreEqual(expected, actual, "Method should return the same object");
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void AddTest()
        {
            AddTestHelper<GenericParameterHelper>();
            AddTestHelper<User>();
        }

        /// <summary>
        ///A test for ActiveSubscriptions`1 Constructor
        ///</summary>
        public void ActiveSubscriptionsConstructorTestHelper<T>()
        {
            ActiveSubscriptions<ISubscriber<T>> target = new ActiveSubscriptions<ISubscriber<T>>();
            Assert.IsNotNull(target, "Did not create an object");
            Assert.IsInstanceOfType(target,typeof(ActiveSubscriptions<ISubscriber<T>>), "did not return the correct type");
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void ActiveSubscriptionsConstructorTest()
        {
            ActiveSubscriptionsConstructorTestHelper<GenericParameterHelper>();
            ActiveSubscriptionsConstructorTestHelper<User>();
        }

        /// <summary>
        ///A test for AddActiveSubscription
        ///</summary>
        public void AddRemoveinParallelTestHelper<T>()
        {
            
            ActiveSubscriptions<T> target = new ActiveSubscriptions<T>(); // TODO: Initialize to an appropriate value
            ActiveSubscriptions<T> specials = TestHelper.GetSpecialSubscribers<T>();
             List<ISubscriber<T>> specialsremove =new List<ISubscriber<T>>();
             List<IMessageStatus<T>> specialsremovestatuses = new List<IMessageStatus<T>>();
            foreach (var item in TestHelper.GetSpecialSubscribers<T>())
            {
                specialsremove.Add(item.Value);
                specialsremovestatuses.Add(new TestSubscriber<T>
                {
                    Id = item.Value.Id
                } as IMessageStatus<T>);
            }
            //List<ISubscriber<T>> specialsremove = (List<ISubscriber<T>>)TestHelper.GetSpecialSubscribers<T>();
            foreach (var item in specials)
            {
                var ret = target.AddActiveSubscription(item.Value);
            }

            target = TestHelper.AddAllotofSubscriptions(target, 1000);
            
            var result = from KeyValuePair<string, ISubscriber<T>>
                         item in target
                         where item.Value.Name == "ZZZ"
                         select item;
             Assert.IsNotNull(result, " did not find the special beforeZZZ");
             result = from KeyValuePair<string, ISubscriber<T>>
                          item in target
                          where item.Value.Name == "XXX"
                          select item;
             Assert.IsNotNull(result, " did not find the special beforeXXX");
             result = from KeyValuePair<string, ISubscriber<T>>
                          item in target
                          where item.Value.Name == "YYY"
                          select item;
             Assert.IsNotNull(result, " did not find the special beforeYYY");

            //var beforeZZZ = target. .First(s => s.Name == "ZZZ");
            //var beforeYYY = target.First(s => s.Name == "YYY");
            //var beforeXXX = target.First(s => s.Name == "XXX");
            //Assert.IsNotNull(beforeXXX, " did not find the special before");
            //Assert.IsNotNull(beforeYYY, " did not find the special beforeYYY");
            //Assert.IsNotNull(beforeZZZ, " did not find the special beforeZZZ");

            bool result3;
            var task1 = Task.Factory.StartNew(()=>
            {
                target = AddTestParallelHelper<T>(target);
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                result3 = target.RemoveIfExists(specialsremovestatuses);
            });

            Task.WaitAll(task1,task2);

            result = from KeyValuePair<string, ISubscriber<T>>
                        item in target
                         where item.Value.Name == "ZZZ"
                         select item;
            Assert.IsNotNull(result, " found the special after it should have been removed eZZZ");
            result = from KeyValuePair<string, ISubscriber<T>>
                         item in target
                     where item.Value.Name == "XXX"
                     select item;
            Assert.IsNotNull(result, "  found the special after it should have been removed XXX");
            result = from KeyValuePair<string, ISubscriber<T>>
                         item in target
                     where item.Value.Name == "YYY"
                     select item;
            Assert.IsNotNull(result, "   found the special after it should have been removed YYY");

            //var afterZZZ = target.Any(s => s.Name == "ZZZ");
            //var afterYYY = target.Any(s => s.Name == "YYY");
            //var afterXXX = target.Any(s => s.Name == "XXX");

            //Assert.IsNull (afterXXX, " found the special after it should have been removed");
            //Assert.IsNull(afterYYY, " found the special after it should have been removed");
            //Assert.IsNull(afterZZZ, " found the special after it should have been removed");

        }

        [TestCategory("UnitTest"), TestMethod()]
        public void AddRemoveinParallelTest()
        {
            AddRemoveinParallelTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for RemoveIfExists
        ///</summary>
        public void RemoveIfExistsTestHelper<T>()
        {
            ActiveSubscriptions<T> target = new ActiveSubscriptions<T>();
            target = AddTestParallelHelper<T>(target);
            List<IMessageStatus<T>> SubscriptionStatus = TestHelper.GetSubscribersStatuses<T>();
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RemoveIfExists(SubscriptionStatus);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(0, target.Count, "Did not remove all objects in list");
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void RemoveIfExistsTest()
        {
            RemoveIfExistsTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for ExpireOldSubscriptions
        ///</summary>
        public void ExpireOldSubscriptionsTestHelper<T>()
        {
            ActiveSubscriptions<T> target = new ActiveSubscriptions<T>(); // TODO: Initialize to an appropriate value
            TestHelper.AddAllotofSubscriptions(target, 1000);
            DateTime now = DateTime.Now;
            int i = 0;
            foreach (var item in target)
            {
                item.Value.StartTime = now;
                long result;
                Math.DivRem(i,2, out result);
                if (result == 0)
                {
                    item.Value.TimeToExpire = new TimeSpan(0);
                }
                i++;
            }
            now = DateTime.Now;

            target.ExpireOldSubscriptions();
            int j = 0;
            foreach (var item in target)
            {
                bool ShouldBeAborted = false;
                var subscriber = item.Value;
                long result2;
                Math.DivRem(j, 2, out result2);
                if (result2 == 0)
                {
                    ShouldBeAborted = true;
                }
                else 
                {
                    ShouldBeAborted = false;
                }

                if (subscriber.Aborted)
                {
                    Assert.IsTrue(ShouldBeAborted, "Was not aborted when it should have been");
                    Assert.IsTrue(subscriber.Aborted, "Did not abort correctly");
                }
                else
                {
                    Assert.IsTrue(DateTime.Compare(subscriber.ExpireTime, now) >= 0, "Did not abort when it should have");
                }
                j++;
            }
        }

        [TestCategory("UnitTest"), TestMethod()]
        public void ExpireOldSubscriptionsTest()
        {
            ExpireOldSubscriptionsTestHelper<GenericParameterHelper>();
        }
    }
}
