using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;

namespace IntegrationTests
{
    [TestClass]
    public class AssemblyLocatorTests
    {
        [TestMethod]
        public void GetAssemblies()
        {
            var assemblies = AssemblyLocator<dummy>.AllDlls;
            Assert.IsNotNull(assemblies);
            Assert.IsTrue(assemblies.Count > 0);
        }

        [TestMethod]
        public void GetTypesInBin()
        {
            var types = AssemblyLocator<dummy>.SubscribersInBin;
            Assert.IsNotNull(types);
            Assert.IsTrue(types.Count > 0);
        }

        [TestMethod]
        public void GetMultipleTypesInBin()
        {
            var types = AssemblyLocator<dummy>.SubscribersInBin;
            var types2 = AssemblyLocator<User>.SubscribersInBin;
            Assert.AreNotEqual(types, types2);
            //Assert.IsTrue(types.Count == 1);
            //Assert.IsTrue(types2.Count == 0);
        }

        [TestMethod]
        public void GetTypesInBinwithGenericParameter()
        {
            GetTypesInBinWithGenericParameterHelper<dummy>();
        }

        private static void GetTypesInBinWithGenericParameterHelper<T>()
        {
            var types = AssemblyLocator<T>.SubscribersInBin;
            Assert.IsNotNull(types);
            Assert.IsTrue(types.Count > 0);
        }
    }

    public class dummy
    {
    }
    public class TestSub<dummy> : Subscriber<dummy>
    {
        public override bool Process(dummy input)
        {
            throw new NotImplementedException();
        }
    }
}
