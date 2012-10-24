using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using Microsoft.Isam.Esent.Interop;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace EsentTests
{
    [TestClass]
    public class EsentTest
    {
        [TestMethod]
        public void ConfigureDatabase()
        {
            var store = new EsentStoreProvider<Dummy>();            
            var databaseName = TestHelper.CleanupName(typeof(Dummy).ToString()) + ".edb";
            TestHelper.VerifyDatabase(databaseName);
        }

        [TestMethod]
        public void DoesDatabaseExistReturnsTrue()
        {
            TestHelper.CreateDatabase("EsentTestsDummy.edb");

            string binFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            IList<string> dllFiles = Directory.GetFiles(binFolder, "*.edb", SearchOption.TopDirectoryOnly).ToList();
            Assert.IsTrue(dllFiles[0].Contains("EsentTestsDummy.edb"));

            var store = new EsentStoreProvider<Dummy>();

            var databaseName = TestHelper.CleanupName(typeof(Dummy).ToString()) + ".edb";
            TestHelper.VerifyDatabase(databaseName);
        }
    }
}
