using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Entities;
using System.Messaging;
using Phantom.PubSub;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for UserManagerServiceAdaptorTest and is intended
    ///to contain all UserManagerServiceAdaptorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserManagerServiceAdaptorTest
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
        ///A test for Create this tests the entire usecase fo a create action
        ///It creates a UserManagerServiceAdaptor that handles the publishing of 
        ///a message to the pubsubchannel, and it save the data to a database
        ///</summary>
        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void CreateTest()
        {
            UserManagerServiceAdaptor target = new UserManagerServiceAdaptor(); 
            User umToUpdate = new User(); 
            umToUpdate.FirstName = "X";
            umToUpdate.LastName = "LastName";
            umToUpdate.UserName = "XLastName2";
            TestHelper.SetUpCleanTestQueue();

            target.Create(umToUpdate);
            //need to assert something
        }

        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void CreateUserUsingDependencyInjectionTest()
        {
            TestHelper.SetUpCleanTestQueue();

            var queueProvider = new MsmqQueueProvider<User>() as IQueueProvider<User>;
            var PubSubChannel = new PublishSubscribeChannel<User>(queueProvider) as IPublishSubscribeChannel<User>;
            PubSubChannel.AddSubscriberType(typeof(TestSubscriber<User>));
            UserManagerServiceAdaptor target = new UserManagerServiceAdaptor(queueProvider, PubSubChannel);
            User umToUpdate = new User();
            umToUpdate.FirstName = "X";
            umToUpdate.LastName = "LastName";
            umToUpdate.UserName = "XLastName1";

            target.Create(umToUpdate);
            //need to assert something
        }


        [TestCategory("IntegrationMsmq"), TestMethod()]
        public void CreateUserUsingConstructorParametersTest()
        {
            TestHelper.SetUpCleanTestQueue();

            var queueProvider = new MsmqQueueProvider<User>() as IQueueProvider<User>;
            var PubSubChannel = new PublishSubscribeChannel<User>(queueProvider) as IPublishSubscribeChannel<User>;
            UserManagerServiceAdaptor target = new UserManagerServiceAdaptor(queueProvider, PubSubChannel);
            User umToUpdate = new User();
            umToUpdate.FirstName = "X";
            umToUpdate.LastName = "LastName";
            umToUpdate.UserName = "XLastName";
            umToUpdate.UserName = "XLastName4";
            target.Create(umToUpdate);
        }
    }
}
