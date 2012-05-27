using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for UserManagerServiceWithInterceptorTest and is intended
    ///to contain all UserManagerServiceWithInterceptorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserManagerServiceWithInterceptorTest
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
        ///A test for SaveUser this doesnt test saving it tests Snap inteception
        ///</summary>
        [TestCategory("Integration"), TestMethod()]
        public void SaveUserWithInterceptorTest()
        {
            Interceptor.SnapConfigurator.Configurator();
            var target = (IUserManagerServiceWithInterceptor)Interceptor.SnapConfigurator._container.Kernel[typeof(IUserManagerServiceWithInterceptor)];

            var target2 = (IUserManagerServiceWithInterceptor)Interceptor.SnapConfigurator._container.Resolve<IUserManagerServiceWithInterceptor>();
            Assert.AreEqual<IUserManagerServiceWithInterceptor>(target, target2, "did not return the correct type");
           // UserManagerServiceWithInterceptor target = new UserManagerServiceWithInterceptor(); // TODO: Initialize to an appropriate value
            string name = "Johnny"; // TODO: Initialize to an appropriate value
            target.SaveUser(name);
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
