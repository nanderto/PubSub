using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;

namespace UnitTests
{
    [TestClass]
    public class ServiceAdaptorTest
    {
        [TestMethod, TestCategory("UnitTest")]
        public void ServiceAdaptorConstructor()
        {
            var adaptor = new TestAdaptor();
            Assert.IsInstanceOfType(adaptor, typeof(TestAdaptor));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Register_1_Filter()
        {
            var adaptor = new TestAdaptor();
            var filter = new TestFilter <Entities.User>();
            var result = adaptor.Register(filter);
            //returns a Ifilter chain of type Entities user
            Assert.IsInstanceOfType(result, typeof(IFilterChain<Entities.User>));
            //the root  should be the filter handed in
            
        }

        [TestMethod, TestCategory("UnitTest")]
        public void Register_2_Filter()
        {
            var adaptor = new TestAdaptor();
            var filter = new TestFilter<Entities.User>();
            var filter2 = new TestFilter<Entities.User>();
            var result = adaptor.Register(filter);
            var result2 = adaptor.Register(filter2);
            adaptor.Execute(new Entities.User());
            //returns a Ifilter chain of type Entities user
            Assert.IsTrue(filter.ProcessCalled);
            Assert.IsTrue(filter2.ProcessCalled);
            //the root  should be the filter handed in

        }
    }

    
    public class TestAdaptor : Phantom.PubSub.ServiceAdaptor<Entities.User>
    {

    }

    public class TestFilter<T> : FilterBase<T>
    {
        public bool ProcessCalled = false;
        protected override T Process(T input)
        {
            ProcessCalled = true;
            //doing nothing in testing
            return input;
        }
    }
}
