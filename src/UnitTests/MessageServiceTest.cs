using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Entities;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for MessageServiceTest and is intended
    ///to contain all MessageServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MessageServiceTest
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
        ///A test for SaveMessage
        ///</summary>
        [TestCategory("Integration"), TestMethod()]
        public void SaveMessageTest()
        {
            MessageService target = new MessageService(); // TODO: Initialize to an appropriate value
            Message input = new Message();
            input.MessageID = "MessageID1";
            input.MessagePutTime = DateTime.Now;
            input.MessageReadTime = DateTime.Now;
            input.Guid = Guid.NewGuid();
            input.Name = "Johnny";
            input.SubscriptionID =  "SubscriptionID1";
            int expected = 1; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.SaveMessage(input);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetMessage
        ///</summary>
        [TestCategory("Integration"), TestMethod()]
        public void GetMessageTest()
        {
            MessageService target = new MessageService(); // TODO: Initialize to an appropriate value


            Message input = new Message();
            input.MessageID = "MessageID1";
            input.MessagePutTime = DateTime.Now;
            input.MessageReadTime = DateTime.Now;
            input.Guid = Guid.NewGuid();
            input.Name = "Johnny";
            input.SubscriptionID = "SubscriptionID1";

            

            var result = target.SaveMessage(input);
            Message actual;
            actual = target.GetMessage(result);
            Assert.AreEqual(1, actual.ID);

            target.DeleteMessage(input);
            actual = target.GetMessage(input.ID);
            Assert.AreEqual(null, actual);
        }
    }
}
