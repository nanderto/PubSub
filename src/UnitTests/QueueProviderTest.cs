using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Messaging;
using System.Collections.Generic;
using Models = Entities;
using BusinessLogic;
using System.Transactions;
using System.Diagnostics;

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

        private static void CleanUpQueues()
        {
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");
            if (MessageQueue.Exists(@".\private$\EntitiesMessage"))
                MessageQueue.Delete(@".\private$\EntitiesMessage");
        }

        private static Models.User GetUserBob()
        {
            Models.User u = new Models.User();
            u.FirstName = "Bob";
            u.LastName = "Johnson";
            u.City = "Hamilton";
            return u;
        }

        private static MessagePacket<Models.User> ReceiveUserPacketFromQueue()
        {
            System.Messaging.Message M = ReceiveMessageFromQueue();
            M.Formatter = new BinaryMessageFormatter();
            var packet = (MessagePacket<Models.User>)M.Body;
            return packet;
        }

        private static Message ReceiveMessageFromQueue()
        {
            MessageQueue msgQ = new MessageQueue(@".\private$\EntitiesUser");
            System.Messaging.Message M = msgQ.Receive(new TimeSpan(1000));
            return M;
        }

        private static MessagePacket<Models.User> GetMessagePacket(Models.User u)
        {

            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = "John",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            var subscribermetadata2 = new SubscriberMetadata()
            {
                Name = "Joe",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            List<ISubscriberMetadata> metadatalist = new List<ISubscriberMetadata>();
            metadatalist.Add(subscribermetadata1);
            metadatalist.Add(subscribermetadata2);

            var message = new MessagePacket<Models.User>(u, metadatalist);
            return message;
        }

        public static string AddAMessage()
        {
            Models.User u = GetUserGwen();

            Message recoverableMessage = new Message();
            recoverableMessage.Body = u;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesUser");

            msgQ.Send(recoverableMessage);
            return recoverableMessage.Id;
        }

        public static void SetUpCleanTestQueueForEntitiesUser()
        {
            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");
            MessageQueue.Create(@".\private$\EntitiesUser");
        }

        private static Models.User GetUserGwen()
        {
            Models.User u = new Models.User();
            u.FirstName = "Gwen";
            u.LastName = "XXXX";
            return u;
        }

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
            Models.User u = GetUserBob();
            string expected = "EntitiesUser"; 
            string actual ;

            //Clean up if exists
            CleanUpQueues();

            actual = target.ConfigureQueue("EntitiesUser", QueueTransactionOption.NoTransactions);
            Assert.AreEqual(expected, actual);

            //Check Name is set correctly
            Assert.AreEqual("EntitiesUser", target.Name, "Name is not set correctly");

            MessageQueue msgQ = new MessageQueue(@".\private$\" + target.Name);
            Assert.IsTrue(msgQ.Transactional);
            //ok it exists now try to create again, should not throw error
            actual = target.ConfigureQueue("EntitiesUser", QueueTransactionOption.NoTransactions);
            Assert.IsTrue(MessageQueue.Exists(@".\private$\EntitiesUser"), " Message queue no longer exists");

            //Clean up if exists
            CleanUpQueues();
        }


        /// <summary>
        ///A test for ConfigureQueue
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ConfigureQueueSupportTransactionsTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = new Models.User();
            string expected = "EntitiesUser";
            string actual;
            CleanUpQueues();

            actual = target.ConfigureQueue("EntitiesUser", QueueTransactionOption.SupportTransactions);
            Assert.AreEqual(expected, actual);

            //Check Name is set correctly
            Assert.AreEqual("EntitiesUser", target.Name, "Name is not set correctly");

            MessageQueue msgQ = new MessageQueue(@".\private$\" + target.Name);
            Assert.IsFalse(msgQ.Transactional);
            //ok it exists now try to create again, should not throw error
            actual = target.ConfigureQueue("EntitiesUser", QueueTransactionOption.SupportTransactions);
            Assert.IsTrue(MessageQueue.Exists(@".\private$\EntitiesUser"), " Message queue no longer exists");

            CleanUpQueues();
        }

        /// <summary>
        ///A test for SaveMessage
        ///</summary>
        [TestMethod(), TestCategory("IntegrationMsmq")]
        public void SaveMessageTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();
            
            var message = GetMessagePacket(u);

            target.PutMessageInTransaction(message);

            //read queue to find the message
            MessageQueue msgQ = new MessageQueue(@".\private$\EntitiesUser");
            System.Messaging.Message M = msgQ.Receive(new TimeSpan(1000));
            M.Formatter = new BinaryMessageFormatter();
            var packet = (MessagePacket<Models.User>)M.Body;

            Assert.IsTrue(packet.Body.FirstName == "Bob", "Did Not return the same message");
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void MSMQPutMessageInTransactionProviderTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();
            var message = GetMessagePacket(u);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    target.PutMessageInTransaction(message);
                    scope.Complete();
                }
            }
            catch(TransactionAbortedException)
            {
                throw;
            }

            var packet = ReceiveUserPacketFromQueue();
            Assert.IsTrue(packet.Body.FirstName == "Bob", "Did Not return the same message");
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void MSMQPutMessageInTransactionProviderAbortedTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();
            var message = GetMessagePacket(u);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    target.PutMessageInTransaction(message);
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException)
            {
                throw;
            }

            var packet = ReceiveUserPacketFromQueue();
            Assert.IsTrue(packet.Body.FirstName == "Bob", "Did Not return the same message");
        }


        /// <summary>
        ///A test for SaveMessage
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void PutMessageInTransactionbutDontuseTransactionTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();

            target.ConfigureQueue("EntitiesUser", QueueTransactionOption.SupportTransactions);
            var message = GetMessagePacket(u);

            target.PutMessageInTransaction(message);

            try
            {
                var packet = ReceiveUserPacketFromQueue();
                Assert.IsTrue(packet.Body.FirstName == "Bob", "Did Not return the same message");
            }
            catch (MessageQueueException ex)
            {
                Trace.WriteLine(ex.ToString());
                Assert.IsFalse(true, "should not have got here, we are not creating a transaction so the method we call will create their own");
            }
            return;
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void PutMessageInTransactionbutDontCommitTransactionTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();

            var message = GetMessagePacket(u);

            try
            {
                var currentTx = System.Transactions.Transaction.Current;
                using (TransactionScope scope = new TransactionScope())
                {
                    target.PutMessageInTransaction(message);
                }
            }
            catch (TransactionAbortedException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(TransactionAbortedException));
            }

            try
            {
                var packet = ReceiveUserPacketFromQueue();
                Assert.IsTrue(packet.Body.FirstName == "Bob", "Did Not return the same message");
            }
            catch (MessageQueueException ex)
            {
                //queue is empty so it throws an exception
                Assert.IsInstanceOfType(ex, typeof(MessageQueueException));
                Assert.IsTrue(ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout);
                return;
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(MessageQueueException));
            }
            //Assert.IsFalse(true, "should not have got here");
            return;
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void WatchQueueTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();

            string actual;
            CleanUpQueues();
            Models.User u = GetUserBob();
            actual = target.ConfigureQueue("EntitiesUser", QueueTransactionOption.SupportTransactions);

           target.SetupWatchQueue(target);
           //well this is a waist of time I need to assert something
        }

        //no longer needed we read queue as a batch only
        //[TestCategory("IntegrationMsmq"), TestMethod()]
        //public void ReadQueueTest()
        //{
        //    IQueueProvider<Models.User> target = CreateQueueProvider();
        //    Models.User u = GetUserGwen();
        //    SetUpCleanTestQueueForEntitiesUser();//Clean up if exists
        //    string recoverableMessageID = AddAMessage();
            
        //    var msgQ = new MessageQueue(@".\private$\EntitiesUser");
        //    target.Name = "EntitiesUser";
        //    target.SetUpWatchQueue(target);
        //    string CorrelationId = string.Empty;
        //    var returnedMessage = target.ReadQueue(out CorrelationId);
            

        //    var message = msgQ.ReceiveById(recoverableMessageID);
        //    Assert.IsNotNull(message, "Message was removed when it should not have been");

        //    message.Formatter =  new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
        //    Models.User returned = (Models.User)message.Body;
        //    Assert.AreEqual(returnedMessage.FirstName, returned.FirstName, "did not return expected message");
        //}

       

        /// <summary>
        ///A test for RemoveFromQueue
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void RemoveFromQueueTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            string CorrelationId = string.Empty; 
            bool actual;
            SetUpCleanTestQueueForEntitiesUser();
            AddAMessage();

            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            Message m = msgQ.Peek();
            
            target.Name = "EntitiesUser";
            target.SetupWatchQueue(target);

            actual = target.RemoveFromQueue(m.Id);
            Assert.IsTrue(actual, "did not retrieve a message");
   
            Message[] messages =  msgQ.GetAllMessages();

            Assert.AreEqual(messages.Length, 0, "Message is still in queue");
        }

        /// <summary>
        ///A test for RemoveFromQueue
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void SaveandRetrieveMessageWithMetadataTest()
        {
            IQueueProvider<Models.User> target = CreateQueueProvider();
            string CorrelationId = string.Empty;
            SetUpCleanTestQueueForEntitiesUser();
            AddAMessage();
            Models.User u = GetUserGwen();

            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = "John",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            var subscribermetadata2 = new SubscriberMetadata()
            {
                Name = "Joe",
                TimeToExpire = new TimeSpan(0, 1, 0)
            };

            List<ISubscriberMetadata> metadatalist = new List<ISubscriberMetadata>();
            metadatalist.Add(subscribermetadata1);
            metadatalist.Add(subscribermetadata2);

            var message = new MessagePacket<Models.User>(u, metadatalist);

            Message recoverableMessage = new Message();
            recoverableMessage.Body = message;
            recoverableMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            recoverableMessage.Recoverable = true;
            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            MessageQueueTransaction msgTx = new MessageQueueTransaction();

            msgQ.Send(recoverableMessage);
           
           
            Message returnMessage = null;
           
            var recoverableMessageID = recoverableMessage.Id;
            returnMessage = msgQ.ReceiveById(recoverableMessageID);
          
       
            Assert.IsNotNull(message, "Message was removed when it should not have been");

            returnMessage.Formatter = new BinaryMessageFormatter(System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways);
            MessagePacket<Models.User> returned = (MessagePacket<Models.User>)returnMessage.Body;

            Assert.IsInstanceOfType(returned, typeof(MessagePacket<Models.User>));

            Assert.AreEqual("Gwen", returned.Body.FirstName);
            Assert.AreEqual("John", returned.SubscriberMetadataList[0].Name);

            Assert.AreEqual("Joe", returned.SubscriberMetadataList[1].Name);
        }

        private void callback(Models.User message, string MessageID)
        {
            ISubscriber<Models.User> m = (ISubscriber<Models.User>)message;
            var now = DateTime.Now;
            
            if (DateTime.Compare(m.ExpireTime, now) < 0)
            {
                Assert.AreEqual("ExpiredSubscription", message.UserName);
            }
            Assert.AreNotEqual("ExpiredSubscription", message.UserName);
        }

    }
}
