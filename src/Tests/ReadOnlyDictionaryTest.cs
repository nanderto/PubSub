using Phantom.PubSub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for ReadOnlyDictionaryTest and is intended
    ///to contain all ReadOnlyDictionaryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReadOnlyDictionaryTest
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

        [TestMethod()]
        public void ReadOnlyDictionaryConstructorTest()
        {
            var dictionary = new Dictionary<string, Tuple<string, Type, TimeSpan>>(); // TODO: Initialize to an appropriate value
            ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>> target = new ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>>(dictionary);
            Assert.IsInstanceOfType(target, typeof(ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>>));
        }

        [TestMethod()]
        public void ReadOnlyDictionaryConstructorwithstuffTest()
        {
            var dictionary = new Dictionary<string, Tuple<string, Type, TimeSpan>>(); 
            dictionary.Add("John", new Tuple<string, Type, TimeSpan>("John", typeof(string), new TimeSpan(0,1,0)));
            ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>> target = new ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>>(dictionary);
            Assert.IsInstanceOfType(target, typeof(ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>>));

        }

        [TestMethod()]
        public void ReadOnlyDictionaryWithStuffRetrievedTest()
        {
            var dictionary = new Dictionary<string, Tuple<string, Type, TimeSpan>>(); 
            var input = new Tuple<string, Type, TimeSpan>("John", typeof(string), new TimeSpan(0, 1, 0));
            dictionary.Add("John", input);
            ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>> target = new ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>>(dictionary);
            Assert.IsInstanceOfType(target, typeof(ReadOnlyDictionary<string, Tuple<string, Type, TimeSpan>>));
            var actual = new Tuple<string, Type, TimeSpan>("", typeof(string), new TimeSpan());
            target.TryGetValue("John", out actual);
            Assert.AreEqual(input, actual);
        }

    }
}
