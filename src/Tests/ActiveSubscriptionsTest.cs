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

        void sub_OnProcessCompletedEvent(object sender, ProcessCompletedEventArgs e)
        {

        }

        void sub_OnProcessStartedEvent(object sender, ProcessStartedEventArgs e)
        {
            var current = e.CurrentSubscription as ISubscriber<GenericParameterHelper>;
            current.StartedProcessing = true;
        }

    }
}
