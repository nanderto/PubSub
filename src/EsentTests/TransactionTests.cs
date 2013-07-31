using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System.Diagnostics;
using Phantom.PubSub;
using System.Globalization;

namespace EsentTests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void Explore1()
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var rm = new EsentResourceManager<Dummy>();
                    if (Transaction.Current != null)
                    {
                        Debug.Assert(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active);
                        Enlistment enlistment = Transaction.Current.EnlistVolatile(rm, EnlistmentOptions.None);
                        rm.DoSomething(1);
                    }

                    var rm2 = new EsentResourceManager<Dummy>();
                    
                    Debug.Assert(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active);
                    Enlistment enlistment2 = Transaction.Current.EnlistVolatile(rm2, EnlistmentOptions.None);
                    rm.DoSomething(3);
                    
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                Debug.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Debug.WriteLine("ApplicationException Message: {0}", ex.Message);
            }

        }

        

        [TestMethod, TestCategory("IntegrationEsent")]
        public void Explore2()
        {
            var messagePacket = TestHelper.BuildAMessage<Dummy>(new Dummy { Id = 12, Name = "MyTestDummy" })
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false).GetMessage();

            Explore2Helper<Dummy>(messagePacket);
        }

        public void Explore2Helper<T>(MessagePacket<T> messagePacket)
        {
            string messageId = string.Empty;
            
            if (!EsentConfig.DoesDatabaseExist("EsentTestsDummy"))
            {
                EsentConfig.CreateDatabaseandMessageStore("EsentTestsDummy");
            }

            try
            {
                using (var store = new EsentStore<T>(true)) 
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        var rm = new EsentStoreResourceManager<T>(store);
                        
                        Repository<T> repository = new Repository<T>(rm);
                        messageId = repository.AddMessage(messagePacket).ToString(CultureInfo.CurrentCulture);
                        scope.Complete();
                    }
                }
            }
            catch (TransactionAbortedException ex)
            {
                Debug.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Debug.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
        }

        [TestMethod]
        public void Explore3TransactionManagement()
        {
            var messagePacket = TestHelper.BuildAMessage<Dummy>(new Dummy { Id = 12, Name = "MyTestDummy" })
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false).GetMessage();

            Explore3Helper<Dummy>(messagePacket);
        }

        public void Explore3Helper<T>(MessagePacket<T> messagePacket)
        {
            string messageId = string.Empty;

            if (!EsentConfig.DoesDatabaseExist("EsentTestsDummy.edb"))
            {
                EsentConfig.CreateDatabaseandMessageStore("EsentTestsDummy.edb");
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var store = new EsentStore<T>(true);
                    using (TransactionScope scope2 = new TransactionScope(TransactionScopeOption.Required))
                    {
                        var rm = new EsentStoreResourceManager<T>(store);
                        
                        Repository<T> repository = new Repository<T>(rm);
                        messageId = repository.AddMessage(messagePacket).ToString(CultureInfo.CurrentCulture);

                        scope2.Complete();
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                Debug.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Debug.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
        }

        [TestMethod]
        public void Explore4TransactionManagement()
        {
            var messagePacket = TestHelper.BuildAMessage<Dummy>(new Dummy { Id = 12, Name = "MyTestDummy" })
                .WithSubscriberMetadataFor(typeof(EsentTestSubscriber<Dummy>), false, false).GetMessage();

            Explore4Helper<Dummy>(messagePacket);
        }

        public void Explore4Helper<T>(MessagePacket<T> messagePacket)
        {
            bool CommitCalled = false;
            bool DisposedCalled = false; 

            string messageId = string.Empty;
            var stubEsentStore = new Phantom.PubSub.Fakes.StubIEsentStore<T>();
            stubEsentStore.Commit = () =>
            {
                CommitCalled = true;
                return;
            };

            stubEsentStore.Dispose = () =>
            {
                DisposedCalled = true;
                return;
            };

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    
                    using (TransactionScope scope2 = new TransactionScope(TransactionScopeOption.Required))
                    {
                        var rm = new EsentStoreResourceManager<T>(stubEsentStore);

                        //Repository<T> repository = new Repository<T>(rm);
                        //messageId = repository.AddMessage(messagePacket).ToString(CultureInfo.CurrentCulture);

                        scope2.Complete();
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                Debug.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Debug.WriteLine("ApplicationException Message: {0}", ex.Message);
            }

            Assert.IsTrue(CommitCalled);
            Assert.IsTrue(DisposedCalled);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void ResourceManagerExceptionthrown()
        {
            bool CommitCalled = false;     
            bool RollbackCalled = false;
            bool DisposedCalled = false;

            string messageId = string.Empty;
            var stubEsentStore = new Phantom.PubSub.Fakes.StubIEsentStore<Dummy>();
            stubEsentStore.Commit = () =>
            {
                CommitCalled = true;
                return;
            };

            stubEsentStore.Rollback = () =>
            {
                RollbackCalled = true;
                return;
            };

            stubEsentStore.Dispose = () =>
            {
                DisposedCalled = true;
                return;
            };

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    using (TransactionScope scope2 = new TransactionScope(TransactionScopeOption.Required))
                    {
                        var rm = new EsentStoreResourceManager<Dummy>(stubEsentStore);
                        //scope not completed
                        //scope2.Complete();
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                Debug.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Debug.WriteLine("ApplicationException Message: {0}", ex.Message);
            }

            Assert.IsFalse(CommitCalled);
            Assert.IsTrue(RollbackCalled);
            Assert.IsTrue(DisposedCalled);
        }

        [TestMethod, TestCategory("UnitTest")]
        public void ResourceManagerExceptionthrown2()
        {
            bool CommitCalled = false;
            bool RollbackCalled = false;
            bool DisposedCalled = false;

            string messageId = string.Empty;
            var stubEsentStore = new Phantom.PubSub.Fakes.StubIEsentStore<Dummy>();
            stubEsentStore.Commit = () =>
            {
                CommitCalled = true;
                return;
            };

            stubEsentStore.Rollback = () =>
            {
                RollbackCalled = true;
                return;
            };

            stubEsentStore.Dispose = () =>
            {
                DisposedCalled = true;
                return;
            };

            try
            {
                using (TransactionScope scope = new TransactionScope())
                { 
                    var rm = new EsentStoreResourceManager<Dummy>(stubEsentStore);
                    //rm.Enlistment.Done();
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                Debug.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            }
            catch (ApplicationException ex)
            {
                Debug.WriteLine("ApplicationException Message: {0}", ex.Message);
            }

            Assert.IsTrue(CommitCalled);
            Assert.IsFalse(RollbackCalled);
            Assert.IsTrue(DisposedCalled);
        }
    }
}
