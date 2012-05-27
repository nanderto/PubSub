using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Messaging;
using System.Collections.Generic;
using Models = Entities;
using BusinessLogic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for QueueProviderTest and is intended
    ///to contain all QueueProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QueueProviderTest
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

        internal virtual IQueueProvider<Models.User> CreateQueueProvider()
        {
            // TODO: Instantiate an appropriate concrete class.
            IQueueProvider<Models.User> target = new MsmqQueueProvider<Models.User>() as IQueueProvider<Models.User>;
            return target;
        }

        /// <summary>
        ///A test for ConfigureQueue
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ConfigureQueueTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = new Models.User();
            string expected = "EntitiesUser"; 
            string actual ;

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");

            actual = target.ConfigureQueue("EntitiesUser");
            Assert.AreEqual(expected, actual);

            //Check Name is set correctly
            Assert.AreEqual("EntitiesUser", target.Name, "Name is not set correctly");

            //ok it exists now try to create again, should not throw error
            actual = target.ConfigureQueue("EntitiesUser");
            Assert.IsTrue(MessageQueue.Exists(@".\private$\EntitiesUser"), " Message queue no longer exists");

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");
        }

        /// <summary>
        ///A test for SaveMessage
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void SaveMessageTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = new Models.User();

            u.FirstName = "Bob";
            target.ConfigureQueue("EntitiesUser");

            target.PutMessage(u);

            Assert.IsTrue(true, "Did not get to end of method.");

            Assert.IsTrue(MessageQueue.Exists(@".\private$\EntitiesUser"), "Message Queue EntitiesUser did not exist");

            MessageQueue msgQ = new MessageQueue(@".\private$\EntitiesUser");
            //read queue to find the message
            System.Messaging.Message M = msgQ.Receive(new TimeSpan(1000));
            M.Formatter = new BinaryMessageFormatter();
            u = (Models.User)M.Body;
            Assert.IsTrue(u.FirstName == "Bob", "Did Not return the same message");
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void WatchQueueTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();

            string expected = "EntitiesUser"; // TODO: Initialize to an appropriate value
            string actual;

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");
            Models.User u = new Models.User();
            u.FirstName = "Bobby";
            actual = target.ConfigureQueue("EntitiesUser");
            Assert.AreEqual(expected, actual);

           // MessageReceiver target = new MessageReceiver();
           // UnitTests.QueueProviderTest.TestGenericMessage testMessage = new UnitTests.QueueProviderTest.TestGenericMessage();
            target.SetUpWatchQueue(target);

        }


        ///// <summary>
        /////A test for GetSubscribers
        /////</summary>
        //[TestCategory("UnitTest"), TestMethod()]
        //public void GetSubscribersTest()
        //{
        //    UserQueueProvider target = new UserQueueProvider(); 
        //    //List<ISubscriber> expected = null; // TODO: Initialize to an appropriate value
        //   User u = new User();

        //    User u2=new User();
        //    List<ISubscriber<User>> actual;
        //   actual = target.GetSubscribers();
        //   // Assert.AreEqual(actual.Count,1, "Did not return expected number of subscribers");
        //}


        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ReadQueueTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = new Models.User();
            u.FirstName = "Gwen";
            //string expected = "EntitiesUser"; // TODO: Initialize to an appropriate value
            //string actual;

            //Clean up if exists
            SetUpCleanTestQueue();
            string recoverableMessageID = AddAMessage();
            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            target.Name = "EntitiesUser";
            target.SetUpWatchQueue(target);
            string CorrelationId = string.Empty;
            var returnedMessage = target.ReadQueue(out CorrelationId);
            

            var message = msgQ.ReceiveById(recoverableMessageID);
            Assert.IsNotNull(message, "Message was removed when it should not have been");

            message.Formatter =  new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            Models.User returned = (Models.User)message.Body;
            Assert.AreEqual(returnedMessage.FirstName, returned.FirstName, "did not return expected message");
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        public static void SetUpCleanTestQueue()
        {
            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");
            MessageQueue.Create(@".\private$\EntitiesUser", true);
        }

        public static string AddAMessage()
        {
            Models.User u = new Models.User();
            u.FirstName = "Gwen";
            u.LastName = "XXXX";

            Message recoverableMessage = new Message();
            recoverableMessage.Body = u;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            MessageQueueTransaction msgTx = new MessageQueueTransaction();
            msgTx.Begin();
            try
            {
                msgQ.Send(recoverableMessage, msgTx);
            }
            catch (Exception)
            {
                msgTx.Abort();
            }
            finally
            {
                msgTx.Commit();
            }
            return recoverableMessage.Id;
        }

        /// <summary>
        ///A test for RemoveFromQueue
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void RemoveFromQueueTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            string CorrelationId = string.Empty; 
            //bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;

            SetUpCleanTestQueue();
            AddAMessage();

            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            Message m = msgQ.Peek();
            target.Name = "EntitiesUser";
            target.SetUpWatchQueue(target);

            actual = target.RemoveFromQueue(m.Id);
            Assert.IsTrue(actual, "did not retrieve a message");

            //makesure queue is empty
           
            Message[] messages =  msgQ.GetAllMessages();

            Assert.AreEqual(messages.Length, 0, "Message is still in queue");
        }
    }
}
