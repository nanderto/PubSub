using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using Entities;
using BusinessLogic;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class AutoConfigTests
    {
        [TestMethod, TestCategory("UnitTest")]
        public void GetSubscriberInfos()
        {
            GetSubscriberInfosHelper<User>();
            GetSubscriberInfosHelper<Message>();
        }


        [TestMethod, TestCategory("UnitTest")]
        public void GetSubscriberInfo_For_The_Type_SubscriberXXX()
        {
            var result = GetSubscriberInfosHelper<Message>();
            Assert.AreEqual("TestMessageSubscriber5", result.Item1);
            Assert.AreEqual(typeof(BusinessLogic.TestMessageSubscriber5).Name, result.Item2.Name);

            //Assert.AreEqual(result.Item2, typeof(BusinessLogic.TestSubscriberXXX<Entities.Message>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void CheckDefaultTimeTOExpireOverride()
        {
            var target = new TestSubscriberXXX<User>();
            Assert.AreEqual(20, target.DefaultTimeToExpire.TotalSeconds);

            //var obj = Activator.CreateInstance(typeof(TestMessageSubscriber), BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding, null, new Object[] { Type.Missing }, null); 
        }

        [TestMethod, TestCategory("UnitTest")]
        public void GetSubscriberInfo_For_The_Type_SubscriberXXXWithTimeToExpire()
        {
            

            var result = GetSubscriberInfosHelper<Message>();
            Assert.AreEqual(result.Item1, "TestMessageSubscriber5");
            Assert.AreEqual(result.Item2.Name, typeof(BusinessLogic.TestMessageSubscriber5).Name);
            Assert.AreEqual(20, result.Item3.TotalSeconds);
            //Assert.AreEqual(result.Item2, typeof(BusinessLogic.TestSubscriberXXX<Entities.Message>));
        }

        public Tuple<string, Type, TimeSpan> GetSubscriberInfosHelper<T>()
        {
            Tuple<string, Type, TimeSpan> returnValue = null;
            var result = AutoConfig<T>.SubscriberInfos;
            foreach (var item in result)
            {
                Assert.IsInstanceOfType(item, typeof(Tuple<string, Type, TimeSpan>));
                returnValue = item;
            }
            return returnValue;
        }
    }
}
