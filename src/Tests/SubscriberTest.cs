using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SubscriberTest and is intended
    ///to contain all SubscriberTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubscriberTest
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
        public void CanProcessAbortedTestHelper<T>()
        {
           // int AbortCount = 0;

            Subscriber<T> target = CreateSubscriber<T>();
            //target.MessageStatusTracker = new MessageStatusTracker<T>();
            target.Abort();
            //abort count is 1 so will not reprocess until 2 mins

            bool expected = false;
            
            bool actual = target.CanProcess();
            Assert.AreEqual(expected, actual);
        }

        public void CanProcessAbortedsleep2minsTestHelper<T>()
        {
            // int AbortCount = 0;

            Subscriber<T> target = CreateSubscriber<T>();
            //target.MessageStatusTracker = new MessageStatusTracker<T>();
            target.Abort();
            //abort count is 1 so will not reprocess until 2 mins
            Thread.Sleep(new TimeSpan(0, 2, 10));
            bool expected = true;

            bool actual = target.CanProcess();
            Assert.AreEqual(expected, actual);
        }
        internal virtual Subscriber<T> CreateSubscriber<T>()
        {
            // TODO: Instantiate an appropriate concrete class.
            Subscriber<T> target = new TestSubscriber<T>();
            return target;
        }

        [TestMethod, TestCategory("UnitTest")]
        public void CanProcessAbortedTest()
        {
            CanProcessAbortedTestHelper<GenericParameterHelper>();
        }

        [TestMethod, TestCategory("SlowTest")]
        public void CanProcessAbortedsleep2minsTest()
        {
            CanProcessAbortedsleep2minsTestHelper<GenericParameterHelper>();
        }
    }
}
