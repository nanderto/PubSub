using System;
using BusinessLogic;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;

namespace UnitTests
{
    [TestClass]
    public class BatchProcessorTests
    {
        [TestMethod, TestCategory("UnitTest")]
        public void BatchProcessorHasStarted()
        {
            Assert.IsTrue(BatchProcessor<User>.HasStarted);
        }

        [TestMethod, TestCategory("IntegrationTest")]
        public void ConfigureBatchProcessor()
        {
            var pubsub = new PublishSubscribeChannel<User>(new MsmqStoreProvider<User>())
               .AddSubscriberType(typeof(TestSubscriberZZZ<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
               .AddSubscriberType(typeof(TestSubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 1, 0));
            BatchProcessor<User>.ConfigureWithPubSubChannel(pubsub);
            Assert.IsTrue(BatchProcessor<User>.IsConfigured);
        }
    }
}
