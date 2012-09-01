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

        //[TestCategory("UnitTest"), TestMethod()]
        //public void ReprocessParallelTest()
        //{
        //    ReprocessTestParallelHelper<GenericParameterHelper>();
        //}

        [TestCategory("UnitTest"), TestMethod()]
        public void ReprocessTestAbortedSubscriber()
        {
            ReprocessTestAbortedSubscriberHelper<GenericParameterHelper>();
        }


        public void ReprocessTestAbortedSubscriberHelper<T>()
        {
            //create SUT
            //ActiveSubscriptionsDictionary<T> target = new ActiveSubscriptionsDictionary<T>();
            //string SubscriptionId = "XXXXXXXXX";
            //T message = default(T);
            //string MessageId = "YYY";
            //List<IMessageStatus<T>> trackIfStarted = TestHelper.GetSubscriptionsStatusTrackers<T>();
            //bool expected = true;
            //bool actual;

            //ISubscriber<T> ActiveSubscription = new TestSubscriber<T>() as ISubscriber<T>;
            //ISubscriber<T> newActiveSubscription = ActiveSubscription;

            
            //ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
            //sub.Name = "jackie";
            //sub.Id = "XXXXXXXXX";
            //sub.TimeToExpire = new TimeSpan(10000);
            //sub.StartTime = DateTime.Now;
            //sub.OnProcessStartedEvent += new OnProcessStartedEvent(sub_OnProcessStartedEvent);
            //sub.OnProcessCompletedEvent += new OnProcessCompletedEvent(sub_OnProcessCompletedEvent);
            //target.AddActiveSubscription(sub);
            //sub.MessageStatusTracker = trackIfStarted[0];
            //sub.Abort();

            //actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
            //expected = CanProcess<T>(sub, sub.AbortedTime + sub.TimeToExpire);

            //actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
            //Assert.AreEqual(expected, actual, "should return true because abort was set to true");

            ////AddTest a bunch more
            //foreach (ISubscriber<T> item in TestHelper.GetSubscribers<T>())
            //{
            //    target.AddActiveSubscription(item);
            //}

            //ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
            //sub2.Name = "jackie";
            //sub2.Id = "XXXXXXXXX";
            //sub2.TimeToExpire = new TimeSpan(10000);
            //sub2.OnProcessStartedEvent += new OnProcessStartedEvent(sub_OnProcessStartedEvent);
            //sub2.OnProcessCompletedEvent += new OnProcessCompletedEvent(sub_OnProcessCompletedEvent);
            //target.AddActiveSubscription(sub);
            //actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
            //expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
            //Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

            //actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
            //expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
            //Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

        }

        /// <summary>
        ///A test for Reprocess
        ///</summary>
        //public void ReprocessTestHelper<T>()
        //{
        //    ActiveSubscriptionsDictionary<T> target = new ActiveSubscriptionsDictionary<T>(); 
        //    string SubscriptionId = "XXXXXXXXX";
        //    T message = default(T); 
        //    string MessageId = "YYY";
        //    List<IMessageStatus<T>> trackIfStarted = TestHelper.GetSubscriptionsStatusTrackers<T>(); 
        //    bool expected = false; 
        //    bool actual;
            
        //    ISubscriber<T> ActiveSubscription = new TestSubscriber<T>() as ISubscriber<T>;
        //    ISubscriber<T> newActiveSubscription = ActiveSubscription;

        //    actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0],trackIfStarted);
        //    Assert.AreEqual(expected, actual, "should return false because the there are none in the ActiveSubscriptionsDictionary collection");

        //    ISubscriber<T> sub = new TestSubscriber<T>() as ISubscriber<T>;
        //    sub.Name = "jackie";
        //    sub.Id = "XXXXXXXXX";
        //    sub.TimeToExpire = new TimeSpan(10000);
        //    sub.OnProcessStartedEvent += new OnProcessStartedEvent(sub_OnProcessStartedEvent);
        //    sub.OnProcessCompletedEvent += new OnProcessCompletedEvent(sub_OnProcessCompletedEvent);
        //    target.AddActiveSubscription(sub); 
        //    actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
        //    expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
        //    Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

        //    //AddTest a bunch more
        //    foreach (ISubscriber<T> item in TestHelper.GetSubscribers<T>())
        //    {
        //        target.AddActiveSubscription(item);
        //    }

        //    ISubscriber<T> sub2 = new TestSubscriber<T>() as ISubscriber<T>;
        //    sub2.Name = "jackie";
        //    sub2.Id = "XXXXXXXXX";
        //    sub2.TimeToExpire = new TimeSpan(10000);
        //    sub2.OnProcessStartedEvent += new OnProcessStartedEvent(sub_OnProcessStartedEvent);
        //    sub2.OnProcessCompletedEvent += new OnProcessCompletedEvent(sub_OnProcessCompletedEvent);
        //    target.AddActiveSubscription(sub);
        //    actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
        //    expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
        //    Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

        //    actual = target.Reprocess(SubscriptionId, message, MessageId, trackIfStarted[0], trackIfStarted);
        //    expected = CanProcess<T>(sub, sub.StartTime + sub.TimeToExpire);
        //    Assert.AreEqual(expected, actual, "because reprocess should get called and it should run sucessfully");

        //}

        void sub_OnProcessCompletedEvent(object sender, ProcessCompletedEventArgs e)
        {

        }

        void sub_OnProcessStartedEvent(object sender, ProcessStartedEventArgs e)
        {
            var current = e.CurrentSubscription as ISubscriber<GenericParameterHelper>;
            current.StartedProcessing = true;
        }

        public void AddTestParallelHelper<T>()
        {
            List<ISubscriber<T>> subscribers = TestHelper.GetSubscribers<T>();
            ActiveSubscriptionsDictionary<T> target = new ActiveSubscriptionsDictionary<T>();
            Parallel.ForEach<ISubscriber<T>>(subscribers, (ISubscriber<T> subscriber) =>
            {
                target.AddActiveSubscription(subscriber);
            });

            Assert.IsTrue(target.Count == 3, " did not return the expected number");
           // Assert.AreEqual(expected, actual, "Method should return the same object");
        }

        public ActiveSubscriptionsDictionary<T> AddTestParallelHelper<T>( ActiveSubscriptionsDictionary<T> SubscriptionList)
        {
            List<ISubscriber<T>> subscribers = TestHelper.GetSubscribers<T>();
            ActiveSubscriptionsDictionary<T> target = SubscriptionList;
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
            ActiveSubscriptionsDictionary<T> target = new ActiveSubscriptionsDictionary<T>();
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
        ///A test for ActiveSubscriptionsDictionary`1 Constructor
        ///</summary>
        public void ActiveSubscriptionsConstructorTestHelper<T>()
        {
            ActiveSubscriptionsDictionary<ISubscriber<T>> target = new ActiveSubscriptionsDictionary<ISubscriber<T>>();
            Assert.IsNotNull(target, "Did not create an object");
            Assert.IsInstanceOfType(target,typeof(ActiveSubscriptionsDictionary<ISubscriber<T>>), "did not return the correct type");
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
            
            ActiveSubscriptionsDictionary<T> target = new ActiveSubscriptionsDictionary<T>(); // TODO: Initialize to an appropriate value
            ActiveSubscriptionsDictionary<T> specials = TestHelper.GetSpecialSubscribers<T>();
             Subscribers<T> specialsremove = new Subscribers<T>();
             //List<IMessageStatus<T>> specialsremovestatuses = new List<IMessageStatus<T>>();
            foreach (var item in TestHelper.GetSpecialSubscribers<T>())
            {
                specialsremove.Add(item.Value);
                //specialsremovestatuses.Add(new MessageStatusTracker<T>
                //{
                //    Id = item.Value.Id
                //} as MessageStatusTracker<T>);
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
             Assert.IsNotNull(result, " did not find the special before ZZZ");
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
                result3 = target.RemoveIfExists(specialsremove);
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
            ActiveSubscriptionsDictionary<T> target = new ActiveSubscriptionsDictionary<T>();
            target = AddTestParallelHelper<T>(target);
            var Subscriptions = TestHelper.GetSubscribers<T>();
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RemoveIfExists(Subscriptions);
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
            ActiveSubscriptionsDictionary<T> target = new ActiveSubscriptionsDictionary<T>(); // TODO: Initialize to an appropriate value
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
