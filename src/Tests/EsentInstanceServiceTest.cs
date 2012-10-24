using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using Ninject;
using Entities;
using TestUtils;

namespace UnitTests
{
    [TestClass]
    public class EsentInstanceServiceTest
    {
        [TestMethod, TestCategory("UnitTest")]
        public void NinjectEsentConstructionTest()
        {
            IKernel kernel = new StandardKernel();
         
            kernel.Bind<Store<User>>().ToSelf().InSingletonScope();
            kernel.Bind<IStoreProvider<User>>().To<EsentStoreProvider<User>>();

            var store = kernel.Get<Store<User>>();
            var store2 = kernel.Get<Store<User>>();
            Assert.AreSame(store, store2);

            kernel.Bind<IEsentStore<User>>().To<EsentStore<User>>();
            kernel.Bind<IPublishSubscribeChannel<User>>().To<PublishSubscribeChannel<User>>();
            
            var samurai = kernel.Get<IStoreProvider<User>>();
            Assert.IsInstanceOfType(samurai, typeof(EsentStoreProvider<User>));
            Assert.IsInstanceOfType(samurai, typeof(IStoreProvider<User>));

            var publishSubscribeChannel = kernel.Get<IPublishSubscribeChannel<User>>();
            Assert.IsInstanceOfType(publishSubscribeChannel, typeof(PublishSubscribeChannel<User>));
            Assert.IsInstanceOfType(publishSubscribeChannel, typeof(IPublishSubscribeChannel<User>));

            store.Dispose();
        }

        [TestMethod, TestCategory("IntegrationMsmq")]
        public void NinjectMsmqConstructionTest()
        {
            IKernel kernel = new StandardKernel();

            kernel.Bind<Store<User>>().ToSelf().InSingletonScope();
            kernel.Bind<IStoreProvider<User>>().To<MsmqStoreProvider<User>>();
            kernel.Bind<IPublishSubscribeChannel<User>>().To<PublishSubscribeChannel<User>>();

            var reader1 = kernel.Get<Store<User>>();
            var reader2 = kernel.Get<Store<User>>();
            Assert.AreSame(reader1, reader2);

            var samurai = kernel.Get<IStoreProvider<User>>();
            Assert.IsInstanceOfType(samurai, typeof(MsmqStoreProvider<User>));
            Assert.IsInstanceOfType(samurai, typeof(IStoreProvider<User>));

            var publishSubscribeChannel = kernel.Get<IPublishSubscribeChannel<User>>();
            Assert.IsInstanceOfType(publishSubscribeChannel, typeof(PublishSubscribeChannel<User>));
            Assert.IsInstanceOfType(publishSubscribeChannel, typeof(IPublishSubscribeChannel<User>));
        }

        [TestMethod, TestCategory("UnitTest")]
        public void NinjectEsentDisposeTest()
        {
            IKernel kernel = new StandardKernel();

            kernel.Bind<Store<User>>().ToSelf().InSingletonScope();
            kernel.Bind<IStoreProvider<User>>().To<EsentStoreProvider<User>>();
            kernel.Bind<IEsentStore<User>>().To<EsentStore<User>>();
            kernel.Bind<IPublishSubscribeChannel<User>>().To<PublishSubscribeChannel<User>>();

            //need to get a store for duration of application
            var store = kernel.Get<Store<User>>();
            
            var pubSub = kernel.Get<IPublishSubscribeChannel<User>>();

            pubSub.AddSubscriberType(typeof(SpeedySubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            User u = new User();

            //User the channel this will ensure that the Esent Database is connected to and the session is established
            pubSub.PublishMessage(u);

            //disposing store will disopose of EsnetInstance Immediatly - not sure how to test this because testing it is null would recreate the instance
            store.Dispose();
        }

        [TestMethod, TestCategory("UnitTest")]
        public void NinjectEsentUseTest()
        {
            IKernel kernel = new StandardKernel();

            kernel.Bind<Store<User>>().ToSelf().InSingletonScope();
            kernel.Bind<IStoreProvider<User>>().To<EsentStoreProvider<User>>();
            kernel.Bind<IEsentStore<User>>().To<EsentStore<User>>();
            kernel.Bind<IPublishSubscribeChannel<User>>().To<PublishSubscribeChannel<User>>();

            //need to get a store for duration of application
            var store = kernel.Get<Store<User>>();

            var pubSub = kernel.Get<IPublishSubscribeChannel<User>>();

            pubSub.AddSubscriberType(typeof(SpeedySubscriber2<User>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

            User u = new User();

            //User the channel this will ensure that the Esent Database is connected to and the session is established
            pubSub.PublishMessage(u);

            //disposing store will disopose of EsnetInstance Immediatly - not sure how to test this because testing it is null would recreate the instance
            store.Dispose();
        }
    }
}
