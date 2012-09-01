using Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    ///This is a test class for Class1Test and is intended
    ///to contain all Class1Test Unit Tests
    ///</summary>
    [TestClass()]
    public class Class1Test
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
        /// tests entity model gets app list from database
        /// </summary>
        [TestCategory("IntegrationDb"), TestMethod()]
        public void getApplicationsTest()
        {
            Repository target = new Repository();
            List<Entities.aspnet_Applications> apps =  target.GetApplications();
            Assert.IsNotNull(apps, "got nothing back");
            Assert.AreEqual(1, apps.Count, "Expected 1 application to be returned");

        }

        /// <summary>
        /// tests database must have database, database connection in config file, and a usercalled johnq
        /// </summary>
        [TestCategory("Integration"), TestMethod()]
        public void GetUserTest()
        {
            Repository target = new Repository();
            Entities.aspnet_Users user = target.GetUserByUserName("john");
            Assert.AreEqual("john", user.UserName, "dID NOt return the expected viewer");
        }

    }
}
