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


        /// <summary>
        ///A test for CanProcess need to figure ouot how to test some thing that is time driven
        ///</summary>
        [TestMethod, TestCategory("UnitTest")]
        public void CanProcessAbortedTest()
        {
           // int AbortCount = 0;

            SubscriberMetadata target = CreateSubscriberMetadata();
            target.RetryCount = ++target.RetryCount;
            target.FailedOrTimedOutTime = DateTime.Now;
            //abort count is 1 so will not reprocess until 2 mins

            bool expected = false;
            
            bool actual = target.CanProcess();
            Assert.AreEqual(expected, actual);
        }


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

        [TestMethod, TestCategory("SlowTest")]
        public async Task CanProcessAbortedsleep2minsTest()
        {
            SubscriberMetadata target = CreateSubscriberMetadata();
            target.FailedOrTimedOutTime = DateTime.Now;
            target.RetryCount = ++target.RetryCount;

            await Task.Delay(new TimeSpan(0, 2, 10));
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
    }
}
