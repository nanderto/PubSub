using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using System.Collections.Generic;
using System.Linq;

namespace EsentTests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod, TestCategory("UnitTest")]
        public void UpdateRepository_2SubscribersNeitherCompleted()
        {
            var messagePacket = TestHelper.BuildAMessage<Dummy>(new Dummy { Id = 12, Name = "MyTestDummy" })
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false).GetMessage();

            RepositoryHelper<Dummy>(messagePacket);
        }

        private static void RepositoryHelper<T>(MessagePacket<T> message)
        {
            string result = string.Empty;
            var savedMetadata = default(ISubscriberMetadata);
            List<ISubscriberMetadata> metaDataList = TestHelper.BuildAMessage<Dummy>(new Dummy { Id = 12, Name = "MyTestDummy" })
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false)
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber2<Dummy>), false, false)
                .GetMetadataList();
            var matchingMetadata = metaDataList.Single(md => md.Name == typeof(EsentTestSubscriber<Dummy>).Name);

            var store = new Phantom.PubSub.Fakes.StubIEsentStore<T>
            {
                GetMetadataInt32 = (int i) => metaDataList,
                UpdateMetadataISubscriberMetadata = metadata => savedMetadata = metadata
            };
            
            Repository<T> repository = new Repository<T>(store);
            //var serializer = new Serializer();

            repository.UpdateMessage(message);

            Assert.IsNotNull(savedMetadata);
            Assert.AreEqual(matchingMetadata.Id, savedMetadata.Id);
            Assert.AreEqual(matchingMetadata.RetryCount, savedMetadata.RetryCount);
            Assert.AreEqual(matchingMetadata.Completed, true);          
        }

        [TestMethod, TestCategory("UnitTest")]
        public void UpdateRepository_2SubscribersOneCompleted()
        {

            Dummy m = new Dummy { Id = 12, Name = "MyTestDummy" };
            UpdateRepository_2SubscribersOneCompletedHelper<Dummy>(m);
        }

        private static void UpdateRepository_2SubscribersOneCompletedHelper<T>(T m)
        {
            var messagePacket = TestHelper.BuildAMessage<T>(m)
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false).GetMessage();

            string result = string.Empty;
            var savedMetadata = default(ISubscriberMetadata);
            List<ISubscriberMetadata> metaDataList = TestHelper.BuildAMessage<Dummy>(new Dummy { Id = 12, Name = "MyTestDummy" })
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false)
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber2<Dummy>), false, true)
                .GetMetadataList();
            var matchingMetadata = metaDataList.Single(md => md.Name == typeof(EsentTestSubscriber<Dummy>).Name);
            var returnedString = string.Empty;
            var returnedMetaDatas = default(List<ISubscriberMetadata>);

            var store = new Phantom.PubSub.Fakes.StubIEsentStore<T>
            {
                GetMetadataInt32 = (int i) => metaDataList,
                //UpdateMetadataISubscriberMetadata = metadata => savedMetadata = metadata,
                DeleteMessageString = passedInString => returnedString = passedInString,
                DeleteMetadataIEnumerableOfISubscriberMetadata = passedInMetadatas => returnedMetaDatas = passedInMetadatas.ToList()
            };

            Repository<T> repository = new Repository<T>(store);
            //var serializer = new Serializer();

            repository.UpdateMessage(messagePacket);

            //Assert.AreNotEqual(savedMetadata.le);
            Assert.AreEqual(messagePacket.MessageId.ToString(), returnedString);
            Assert.AreEqual(matchingMetadata.Completed, true);
            Assert.IsNotNull(returnedMetaDatas);
            Assert.AreEqual(metaDataList[0].Id, returnedMetaDatas[0].Id);
        }
    }
}
