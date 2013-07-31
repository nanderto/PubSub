using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Messaging;
using System.Collections.Generic;
using Models = Entities;
using BusinessLogic;
using System.Transactions;
using System.Diagnostics;

namespace IntegrationTests
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

        internal virtual IStoreProvider<Models.User> CreateQueueProvider()
        {
            // TODO: Instantiate an appropriate concrete class.
            IStoreProvider<Models.User> target = new MsmqStoreProvider<Models.User>() as IStoreProvider<Models.User>;
            return target;
        }

        [TestMethod, TestCategory("IntegrationTests")]
        public void CreateMSMQProvider()
        {
            var queue = new MsmqStoreProvider<Models.Message>();
            Assert.IsInstanceOfType(queue, typeof(MsmqStoreProvider<Models.Message>));
            Assert.AreEqual("EntitiesMessage", queue.Name);
        }

        /// <summary>
        ///A test for ConfigureQueue
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void ConfigureQueueTest()
        {
            IStoreProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            bool expected = true; 
            bool actual ;

            //Clean up if exists
            CleanUpQueues();       
            
            actual = target.ConfigureStore("EntitiesUser", StoreTransactionOption.NoTransactions);
            Assert.AreEqual(expected, actual);

            var q = UnitTests.TestHelper.FindQueue("EntitiesUser");

            //Check Name is set correctly
            Assert.AreEqual(@".\private$\EntitiesUser", q.Path, "Name is not set correctly");

            //MessageQueue msgQ = new MessageQueue(@".\private$\" + target.Name);
            Assert.IsTrue(!q.Transactional);
            //ok it exists now try to create again, should not throw error
            actual = target.ConfigureStore("EntitiesUser", StoreTransactionOption.NoTransactions);
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
            IStoreProvider<Models.User> target = CreateQueueProvider();
            Models.User u = new Models.User();
            bool expected = true;
            bool actual;
            CleanUpQueues();

            actual = target.ConfigureStore("EntitiesUser", StoreTransactionOption.SupportTransactions);
            Assert.AreEqual(expected, actual);

            //Get the Queue this will throw Exception if Queue does not exist
            var q = UnitTests.TestHelper.FindQueue("EntitiesUser");
            //Check Name is set correctly
            Assert.AreEqual(@".\private$\EntitiesUser", q.Path, "Name is not set correctly");

            Assert.IsTrue(q.Transactional);
            //ok it exists now try to create again, should not throw error
            actual = target.ConfigureStore("EntitiesUser", StoreTransactionOption.SupportTransactions);
            Assert.IsTrue(MessageQueue.Exists(@".\private$\EntitiesUser"), " Message queue no longer exists");

            CleanUpQueues();
        }

        /// <summary>
        ///A test for SaveMessage
        ///</summary>
        [TestMethod(), TestCategory("IntegrationMsmq")]
        public void SaveMessageTest()
        {
            IStoreProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();
            
            var message = GetMessagePacket(u);

            target.PutMessage(message);

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
            IStoreProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();
            var message = GetMessagePacket(u);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    target.PutMessage(message);
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
            IStoreProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();
            var message = GetMessagePacket(u);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    target.PutMessage(message);
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
            IStoreProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();

            target.ConfigureStore("EntitiesUser", StoreTransactionOption.SupportTransactions);
            var message = GetMessagePacket(u);

            target.PutMessage(message);

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
            IStoreProvider<Models.User> target = CreateQueueProvider();
            Models.User u = GetUserBob();
            SetUpCleanTestQueueForEntitiesUser();

            var message = GetMessagePacket(u);

            try
            {
                var currentTx = System.Transactions.Transaction.Current;
                using (TransactionScope scope = new TransactionScope())
                {
                    target.PutMessage(message);
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

        /// <summary>
        ///A test for RemoveFromQueue
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void RemoveFromQueueTest()
        {
            IStoreProvider<Models.User> target = CreateQueueProvider();
            string CorrelationId = string.Empty; 
            bool actual;
            SetUpCleanTestQueueForEntitiesUser();
            AddAMessage();

            var msgQ = new MessageQueue(@".\private$\EntitiesUser");
            Message m = msgQ.Peek();
            
            target.Name = "EntitiesUser";

            actual = target.SubscriberGroupCompletedForMessage(m.Id);
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
            IStoreProvider<Models.User> target = CreateQueueProvider();
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

        //private void callback(Models.User message, string MessageID)
        //{
        //    ISubscriber<Models.User> m = (ISubscriber<Models.User>)message;
        //    var now = DateTime.Now;
            
        //    if (DateTime.Compare(m.ExpireTime, now) < 0)
        //    {
        //        Assert.AreEqual("ExpiredSubscription", message.UserName);
        //    }
        //    Assert.AreNotEqual("ExpiredSubscription", message.UserName);
        //}

    }
}
