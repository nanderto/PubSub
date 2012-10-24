using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using System.Collections.Generic;
using Microsoft.Isam.Esent.Interop;
using System.Diagnostics;

namespace EsentTests
{
    [TestClass]
    public class EsentStoreTests
    {
        [TestMethod, TestCategory("IntegrationEsent")]
        public void AddMessageAndSeperateMetadata()
        {
            var storeprovider = new EsentStoreProvider<Dummy>();
            var dummy = new Dummy()
            {
                Id = 12,
                Name = "Wilson"
            };
            
            List<ISubscriberMetadata> metadatas = TestHelper.GetMetadatas();
            var messagePacket = new MessagePacket<Dummy>(dummy, metadatas);
            
            int result;

            using (var store = new EsentStore<Dummy>(true))
            {
                Repository<Dummy> repository = new Repository<Dummy>(store);
                string body = string.Empty;
                result = repository.AddMessage(messagePacket);
                store.Commit();
            }
            int messageId = result;
            using (var store = new EsentStore<Dummy>())
            {
                Repository<Dummy> repository = new Repository<Dummy>(store);
                var messageList = store.GetMetadata(messageId);
                int metadataId = Convert.ToInt32(messageList[0].Id);
                Trace.WriteLine("Metadata ID returned, I kind of expect it to be 1 because this is a unit test that should have a new database: " + metadataId.ToString());

                ISubscriberMetadata resultSubscriberMetadata = store.GetMetadata(messageId, metadataId);

                Assert.IsInstanceOfType(resultSubscriberMetadata, typeof(ISubscriberMetadata));
                Assert.IsInstanceOfType(resultSubscriberMetadata, typeof(SubscriberMetadata));
                Assert.AreEqual("John", resultSubscriberMetadata.Name);
                Assert.AreEqual(metadataId, Convert.ToInt32(resultSubscriberMetadata.Id));

                //messageId = 1;
                metadataId = Convert.ToInt32(messageList[1].Id);
                Trace.WriteLine("Metadata ID returned, I kind of expect it to be 2 because this is a unit test that should have a new database: " + metadataId.ToString());
                
                resultSubscriberMetadata = store.GetMetadata(messageId, metadataId);

                Assert.IsInstanceOfType(resultSubscriberMetadata, typeof(ISubscriberMetadata));
                Assert.IsInstanceOfType(resultSubscriberMetadata, typeof(SubscriberMetadata));
                Assert.AreEqual("Joe", resultSubscriberMetadata.Name);

                Assert.AreEqual(metadataId, Convert.ToInt32(resultSubscriberMetadata.Id));
            }

            using (var store = new EsentStore<Dummy>())
            {
                Repository<Dummy> repository = new Repository<Dummy>(store);
                List<ISubscriberMetadata> result2 = store.GetMetadata(messageId);

                Assert.IsInstanceOfType(result2[0], typeof(ISubscriberMetadata));
                Assert.IsInstanceOfType(result2[0], typeof(SubscriberMetadata));
                Assert.AreEqual("John", result2[0].Name);                
                Assert.AreEqual(2, result2.Count);
            }
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void GetMetadata_WithMultipleRecords()
        {
            TestHelper.Add4MessagesToStore();
            var storeProvider = new EsentStoreProvider<Dummy>();
            var mp = TestHelper.GetMessagePacket(new Dummy() { Name = "MySpecialMessage", Id = 23 });
            mp.SubscriberMetadataList[0].Name = "XXXX";
            var messageId = storeProvider.PutMessage(mp);

            using (var store = new EsentStore<Dummy>())
            {
                
                Repository<Dummy> repository = new Repository<Dummy>(store);
                //int messageId = 1;
                List<ISubscriberMetadata> result = store.GetMetadata(Convert.ToInt32(messageId));

                Assert.IsInstanceOfType(result[0], typeof(ISubscriberMetadata));
                Assert.IsInstanceOfType(result[0], typeof(SubscriberMetadata));
                Assert.AreEqual("XXXX", result[0].Name);
                
                Assert.AreEqual(2, result.Count);
            }
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void UpdateMetadata_WithMultipleRecords()
        {
            TestHelper.Add4MessagesToStore();
            var storeProvider = new EsentStoreProvider<Dummy>();
            var mp = TestHelper.GetMessagePacket(new Dummy() { Name = "MySpecialMessage", Id = 23 });
            mp.SubscriberMetadataList[0].Name = "XXXX";
            var messageId = storeProvider.PutMessage(mp);
            DateTime then = DateTime.Now;
            DateTime now = DateTime.Now;
            using (var store = new EsentStore<Dummy>(true))
            {

                Repository<Dummy> repository = new Repository<Dummy>(store);
                //int messageId = 1;
                List<ISubscriberMetadata> result = store.GetMetadata(Convert.ToInt32(messageId));
                var metadataId = result[0].Id;
                var subscribermetadata1 = new SubscriberMetadata()
                {
                    Id = metadataId,
                    Name = "Updated",
                    TimeToExpire = new TimeSpan(0, 1, 1),
                    RetryCount = 23,
                    FailedOrTimedOut = true,
                    Completed = true,
                    FailedOrTimedOutTime = now,
                    StartTime = then,
                    MessageId = Convert.ToInt32(messageId)
                };

                store.UpdateMetadata(subscribermetadata1);

                ISubscriberMetadata metadata = store.GetMetadata(Convert.ToInt32(messageId), Convert.ToInt32(metadataId));

                store.Commit();


                Assert.IsInstanceOfType(metadata, typeof(ISubscriberMetadata));
                Assert.IsInstanceOfType(metadata, typeof(SubscriberMetadata));
                Assert.AreEqual("Updated", metadata.Name);
                Assert.AreEqual(23, metadata.RetryCount);
                Assert.AreEqual(messageId, metadata.MessageId.ToString());
            }
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void DeleteMetadata()
        {
            Dummy m = new Dummy { Id = 12, Name = "MyTestDummy" };
            var messagePacket = TestHelper.BuildAMessage<Dummy>(m)
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false).GetMessage();

            var storeprovider = new EsentStoreProvider<Dummy>();

            using (var store = new EsentStore<Dummy>(true))
            {
                var preresults = store.GetAllMessages();
                Repository<Dummy> repository = new Repository<Dummy>(store);
                var result = repository.AddMessage(messagePacket);
                store.Commit();
                List<ISubscriberMetadata> metadatas = store.GetMetadata(Convert.ToInt32(result));
                Assert.AreEqual(1, metadatas.Count);

                using (var transaction = store.OpenTransaction())
                {
                    store.DeleteMetadata(metadatas);
                    transaction.Commit(CommitTransactionGrbit.LazyFlush);
                }


                repository = new Repository<Dummy>(store);
                metadatas = store.GetMetadata(Convert.ToInt32(result));
                Assert.AreEqual(0, metadatas.Count);
            }
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void DeleteMultipleMetadata()
        {
            Dummy m = new Dummy { Id = 12, Name = "MyTestDummy" };
            var messagePacket = TestHelper.BuildAMessage<Dummy>(m)
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber2<Dummy>), false, false)
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false).GetMessage();

            var storeprovider = new EsentStoreProvider<Dummy>();

            //using (var store = new EsentStore<Dummy>())
            //{
            //    //var preresults = store.GetAllMessages();
            //}

            int result;

            using (var store = new EsentStore<Dummy>(true))
            {
                
                Repository<Dummy> repository = new Repository<Dummy>(store);
                result = repository.AddMessage(messagePacket);
                store.Commit();
            }

            List<ISubscriberMetadata> metadatas;
            using(var store = new EsentStore<Dummy>())
            {
                metadatas = store.GetMetadata(Convert.ToInt32(result));
                Assert.AreEqual(2, metadatas.Count);
            }

            using (var store = new EsentStore<Dummy>())
            {
                store.DeleteMetadata(metadatas);
                store.Commit();
            }

            using (var store = new EsentStore<Dummy>())
            {
                //var repository = new Repository<Dummy>(store);
                metadatas = store.GetMetadata(Convert.ToInt32(result));
                Assert.AreEqual(0, metadatas.Count);
            }
        }
    }
}
