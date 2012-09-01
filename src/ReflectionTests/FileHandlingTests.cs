using System;
using System.Reflection;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using System.Diagnostics;
using System.IO;

namespace ReflectionTests
{
    [TestClass]
    public class FileHandlingTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Uri uri = new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
            
           var d = Environment.CurrentDirectory;
//            //var directory = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo di = new DirectoryInfo(uri.LocalPath);
            var dlls = di.GetFiles("*.dll");
           
            Assembly[] appAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in dlls)
            {
                var ass = System.Reflection.Assembly.ReflectionOnlyLoadFrom(item.FullName);

            }
//            foreach (Assembly assembly in appAssemblies)
//            {
//                Debug.WriteLine(assembly.FullName);
//            }
            
////For Each asm As System.Reflection.Assembly In
//// System.AppDomain.CurrentDomain.GetAssemblies()
//// For Each mdl As System.Reflection.Module In asm.GetModules()
//// For Each typ As System.Type In mdl.GetTypes()
//// If typ.IsSubclassOf(GetType(MyBasePage))
 
//            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()
//                            where t.GetInterfaces().Contains(typeof(IFindMe))
//                            && t.GetConstructor(Type.EmptyTypes) != null
//                            select t;
//            foreach (var item in instances)
//            {
//                Assert.AreEqual(typeof(BusinessLogic.TestMessageSubscriber2), item);
                
//            }
        }
    }
}
