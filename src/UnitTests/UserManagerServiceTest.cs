//using Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BusinessLogic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for UserManagerServiceTest and is intended
    ///to contain all UserManagerServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserManagerServiceTest
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
        ///A test for UserManagerService Constructor
        ///</summary>
        [TestCategory("UnitTest"), TestMethod()]
        public void UserManagerServiceConstructorTest()
        {
            //UserManagerService_Accessor target = new UserManagerService_Accessor();
            //Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestCategory("Performance"), TestMethod()]
        public void GetUserbyUserName()
        {
            string UserName = "john";
            BusinessLogic.UserManagerService target = new UserManagerService();
            Entities.aspnet_Users user = target.GetUserByUserName(UserName);
            Assert.AreEqual("john",user.UserName, "Wrong person");
        }
    }
}
