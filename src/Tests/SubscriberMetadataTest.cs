using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SubscriberTest and is intended
    ///to contain all SubscriberTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubscriberMetadataTest
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
        public void CanProcessNotAbortedTest()
        {
            // create a subscriber that has gone past its expirey but has not been expired (aborted)

            SubscriberMetadata target = new SubscriberMetadata()
            {
                StartTime = DateTime.Now - new TimeSpan(0, 1, 5),
                RetryCount = 0,
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            target.RetryCount = 0;
            //target.FailedOrTimedOutTime = DateTime.Now;
            //this  is a subscriber that has expired but it has not had the abort process called on it so we want it toreturn true for can process.

            bool expected = true;

            bool actual = target.CanProcess();
            Assert.AreEqual(expected, actual);
        }
        
        internal virtual SubscriberMetadata CreateSubscriberMetadata()
        {
            SubscriberMetadata target = new SubscriberMetadata()
                {
                    StartTime = DateTime.Now,
                    RetryCount = 0,
                    TimeToExpire = new TimeSpan(0,0,1)
                };
            return target;
        }

        [TestMethod, TestCategory("UnitTest")]
        public void CanProcessRetry1Test()
        {
            SubscriberMetadata target = new SubscriberMetadata()
            {
                StartTime = DateTime.Now,
                RetryCount = 0,
                TimeToExpire = new TimeSpan(0, 0, 1)
            };
            target.FailedOrTimedOutTime = DateTime.Now;
            target.RetryCount = 1;

            var currentTimeProvider = new Phantom.PubSub.Fakes.StubICurrentTimeProvider
            {
                NowGet = () => target.FailedOrTimedOutTime.Add(new TimeSpan(0, 2, 1))
            };

            bool expected = true;

            bool actual = target.CanProcess(currentTimeProvider);
            Assert.AreEqual(expected, actual);

            currentTimeProvider.NowGet = () => target.FailedOrTimedOutTime.Add(new TimeSpan(0, 1, 59));

            expected = false;

            actual = target.CanProcess(currentTimeProvider);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod, TestCategory("UnitTest")]
        public void CanProcessRetry2Test()
        {
            SubscriberMetadata target = new SubscriberMetadata()
            {
                StartTime = DateTime.Now,
                RetryCount = 0,
                TimeToExpire = new TimeSpan(0, 0, 1)
            };
            target.FailedOrTimedOutTime = DateTime.Now;
            target.RetryCount = 2;

            var currentTimeProvider = new Phantom.PubSub.Fakes.StubICurrentTimeProvider
            {
                NowGet = () => target.FailedOrTimedOutTime.Add(new TimeSpan(0, 4, 1))
            };

            bool expected = true;

            bool actual = target.CanProcess(currentTimeProvider);
            Assert.AreEqual(expected, actual);

            currentTimeProvider.NowGet = () => target.FailedOrTimedOutTime.Add(new TimeSpan(0, 3, 59));

            expected = false;

            actual = target.CanProcess(currentTimeProvider);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void CanProcessRetry3Test()
        {
            SubscriberMetadata target = new SubscriberMetadata()
            {
                StartTime = DateTime.Now,
                RetryCount = 0,
                TimeToExpire = new TimeSpan(0, 0, 1)
            };
            target.FailedOrTimedOutTime = DateTime.Now;
            target.RetryCount = 3;

            var currentTimeProvider = new Phantom.PubSub.Fakes.StubICurrentTimeProvider
            {
                NowGet = () => target.FailedOrTimedOutTime.Add(new TimeSpan(0, 8, 1))
            };

            bool expected = true;

            bool actual = target.CanProcess(currentTimeProvider);
            Assert.AreEqual(expected, actual);

            currentTimeProvider.NowGet = () => target.FailedOrTimedOutTime.Add(new TimeSpan(0, 7, 59));

            expected = false;

            actual = target.CanProcess(currentTimeProvider);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void HasExpiredTest()
        {
            SubscriberMetadata target = new SubscriberMetadata()
            {
                StartTime = DateTime.Now,
                RetryCount = 0,
                TimeToExpire = new TimeSpan(0, 0, 1)
            };
            //target.FailedOrTimedOutTime = DateTime.Now;
            target.RetryCount = 1;

            var currentTimeProvider = new Phantom.PubSub.Fakes.StubICurrentTimeProvider
            {
                NowGet = () => target.StartTime.Add(target.TimeToExpire).Add(new TimeSpan(0, 0, 0, 1))
            };

            bool expected = true;

            bool actual = SubscriberMetadata.HasExpired(target, currentTimeProvider);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void HasNotExpiredTest()
        {
            SubscriberMetadata target = new SubscriberMetadata()
            {
                StartTime = DateTime.Now,
                RetryCount = 0,
                TimeToExpire = new TimeSpan(0, 0, 1)
            };
            //target.FailedOrTimedOutTime = DateTime.Now;
            target.RetryCount = 1;

            var currentTimeProvider = new Phantom.PubSub.Fakes.StubICurrentTimeProvider
            {
                NowGet = () => target.StartTime.Add(target.TimeToExpire).Subtract(new TimeSpan(0, 0, 0, 1))
            };

            bool expected = false;

            bool actual = SubscriberMetadata.HasExpired(target, currentTimeProvider);
            Assert.AreEqual(expected, actual);
        }
    }
}
