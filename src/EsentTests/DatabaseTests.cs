using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Isam.Esent.Interop;
using System.Collections.Generic;
using System.Text;
using Phantom.PubSub;

namespace EsentTests
{
    [TestClass]
    public class DatabaseTests
    {
        private static string DatabaseName = "PhantomPubSub.edb";
        private static string TableName = "messages";
        private static string StoreName = "storenametest";

        [TestMethod, TestCategory("IntegrationEsent")]
        public void CreateDatabase()
        {
            TestHelper.CreateDatabase(StoreName);
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void AttachtoDatabaseAndVerifyColumns()
        {
            var store = new EsentStoreProvider<Dummy>();
            TestHelper.VerifyDatabase("EsentTestsDummy");
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void InsertOneMessage()
        {
            TestHelper.CreateDatabase("EsentTestsDummy");
            using (var instance = new Instance("MyInstanceName"))
            {
                instance.Parameters.CircularLog = true;
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetAttachDatabase(session, DatabaseName, AttachDatabaseGrbit.None);
                    Api.JetOpenDatabase(session, DatabaseName, null, out dbid, OpenDatabaseGrbit.None);

                    JET_TABLEID tableid;

                    if (Api.TryOpenTable(session, dbid, "EsentTestsDummymessage", OpenTableGrbit.None, out tableid))
                    {
                        IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, tableid);
                        JET_COLUMNID columnidMessage = columnids["message"];
                        JET_COLUMNID columnidMetaData = columnids["metadata"];

                        Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                        Assert.IsInstanceOfType(columnidMetaData, typeof(JET_COLUMNID));
                        AddMessage(session, tableid, ref columnidMessage, ref columnidMetaData);
                    }
                    else
                    {
                        using (var table = new Table(session, dbid, "EsentTestsDummymessage", OpenTableGrbit.None))
                        {
                            IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, table);
                            JET_COLUMNID columnidMessage = columnids["message"];
                            JET_COLUMNID columnidMetaData = columnids["metadata"];

                            Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                            Assert.IsInstanceOfType(columnidMetaData, typeof(JET_COLUMNID));
                            AddMessage(session, table, ref columnidMessage, ref columnidMetaData);
                        }
                    }                  
                }
            }           
        }

        private static void AddMessage(Session session, JET_TABLEID table, ref JET_COLUMNID columnidMessage, ref JET_COLUMNID columnidMetaData)
        {
            using (var transaction = new Transaction(session))
            {
                using (var update = new Update(session, table, JET_prep.Insert))
                {
                    var message = "Hi this is the message";
                    var metadata = "Hi this is the Metadata";

                    Api.SetColumn(session, table, columnidMessage, message, Encoding.Unicode);
                    Api.SetColumn(session, table, columnidMetaData, metadata, Encoding.Unicode);


                    // Save the update at the end of the using block!
                    // If update.Save isn't called then the update will 
                    // be canceled when disposed (and the record won't
                    // be inserted).
                    //
                    // Inserting a record does not change the location of
                    // the cursor (JET_TABLEID); it will have the same
                    // location that it did before the insert.
                    // To insert a record and then position the cursor
                    // on the record, use Update.SaveAndGotoBookmark. That
                    // call uses the bookmark returned from JetUpdate to
                    // position the tableid on the new record.
                    update.Save();
                }

                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

        [TestMethod, TestCategory("IntegrationEsent")]
        public void IinsertAndReadOneMessage()
        {
            DatabaseName = "EsentTestsDummy";
            TestHelper.CreateDatabase("EsentTestsDummy", "EsentTestsDummy");
            using (var instance = new Instance("MyInstanceName"))
            {
                instance.Parameters.CircularLog = true;
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetAttachDatabase(session, DatabaseName, AttachDatabaseGrbit.None);
                    Api.JetOpenDatabase(session, DatabaseName, null, out dbid, OpenDatabaseGrbit.None);

                    JET_TABLEID tableid;

                    if (Api.TryOpenTable(session, dbid, "EsentTestsDummymessages", OpenTableGrbit.None, out tableid))
                    {
                        IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, tableid);
                        JET_COLUMNID columnidMessage = columnids["message"];
                        JET_COLUMNID columnidMetaData = columnids["metadata"];

                        Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                        Assert.IsInstanceOfType(columnidMetaData, typeof(JET_COLUMNID));
                        AddMessage(session, tableid, ref columnidMessage, ref columnidMetaData);

                        var results = TestHelper.DumpByIndex(session, tableid, null, columnids);
                        Assert.IsNotNull(results);
                        Assert.IsTrue(results.Count == 1);
                        Assert.AreEqual("Hi this is the message", results[0]);
                    }
                    else
                    {
                        using (var table = new Table(session, dbid, "EsentTestsDummymessages", OpenTableGrbit.None))
                        {
                            IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, table);
                            JET_COLUMNID columnidMessage = columnids["message"];
                            JET_COLUMNID columnidMetaData = columnids["metadata"];

                            Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                            Assert.IsInstanceOfType(columnidMetaData, typeof(JET_COLUMNID));
                            AddMessage(session, table, ref columnidMessage, ref columnidMetaData);

                            var results = TestHelper.DumpByIndex(session, table, null, columnids);
                            Assert.IsNotNull(results);
                            Assert.IsTrue(results.Count == 1);
                            Assert.AreEqual("Hi this is the message", results[0]);
                        }
                    }                  
                }
            }

        }



        [TestMethod, TestCategory("IntegrationEsent")]
        public void InsertRecordRetrieveAutoIncrement()
        {
            TestHelper.CreateDatabase(DatabaseName);
            using (var instance = new Instance("MyInstanceName"))
            {
                instance.Parameters.CircularLog = true;
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetAttachDatabase(session, DatabaseName, AttachDatabaseGrbit.None);
                    Api.JetOpenDatabase(session, DatabaseName, null, out dbid, OpenDatabaseGrbit.None);

                    JET_TABLEID tableid;

                    if (Api.TryOpenTable(session, dbid, "messages", OpenTableGrbit.None, out tableid))
                    {
                        IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, tableid);
                        JET_COLUMNID autoincColumn = columnids["id"];
                        JET_COLUMNID columnidMessage = columnids["message"];
                        JET_COLUMNID columnidMetaData = columnids["metadata"];
                        using (var transaction = new Transaction(session))
                        {
                            using (var update = new Update(session, tableid, JET_prep.Insert))
                            {
                                var message = "Hi this is the message";
                                var metadata = "Hi this is the Metadata";

                                Api.SetColumn(session, tableid, columnidMessage, message, Encoding.Unicode);
                                Api.SetColumn(session, tableid, columnidMetaData, metadata, Encoding.Unicode);

                                int? autoinc = Api.RetrieveColumnAsInt32(
                                    session,
                                    tableid,
                                    autoincColumn,
                                    RetrieveColumnGrbit.RetrieveCopy);
                                update.Save();
                                Assert.IsTrue(autoinc != null || autoinc != 0);
                            }

                            transaction.Commit(CommitTransactionGrbit.LazyFlush);
                        }

                        using (var transaction = new Transaction(session))
                        {
                            SeekToId(session, tableid, 1);
                            string result = Api.RetrieveColumnAsString(session, tableid, columnidMessage);
                            Assert.AreEqual("Hi this is the message", result);
                            transaction.Commit(CommitTransactionGrbit.LazyFlush);
                        }
                    }
                    else
                    {
                        using (var table = new Table(session, dbid, TableName, OpenTableGrbit.None))
                        {
                            IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, table);
                            JET_COLUMNID autoincColumn = columnids["id"];
                            JET_COLUMNID columnidMessage = columnids["message"];
                            JET_COLUMNID columnidMetaData = columnids["metadata"];
                            using (var transaction = new Transaction(session))
                            {
                                using (var update = new Update(session, table, JET_prep.Insert))
                                {
                                    var message = "Hi this is the message";
                                    var metadata = "Hi this is the Metadata";

                                    Api.SetColumn(session, table, columnidMessage, message, Encoding.Unicode);
                                    Api.SetColumn(session, table, columnidMetaData, metadata, Encoding.Unicode);

                                    int? autoinc = Api.RetrieveColumnAsInt32(
                                        session,
                                        table,
                                        autoincColumn,
                                        RetrieveColumnGrbit.RetrieveCopy);
                                    update.Save();
                                    Assert.IsTrue(autoinc != null || autoinc != 0);
                                }
                                
                                transaction.Commit(CommitTransactionGrbit.LazyFlush);
                            }

                            using (var transaction = new Transaction(session))
                            {
                                SeekToId(session, table, 1);
                                string result = Api.RetrieveColumnAsString(session, table, columnidMessage);
                                Assert.AreEqual("Hi this is the message", result);
                                transaction.Commit(CommitTransactionGrbit.LazyFlush);
                            }
                        }
                    }
                }
            }
            


        }

        private static void SeekToId(JET_SESID sesid, JET_TABLEID tableid, int id)
        {
            // We need to be on the primary index (which indexes the 'symbol' column).
            Api.JetSetCurrentIndex(sesid, tableid, null);
            Api.MakeKey(sesid, tableid, id, MakeKeyGrbit.NewKey);

            // This seek expects the record to be present. To test for a record
            // use TrySeek(), which won't throw an exception if the record isn't
            // found.
            Api.JetSeek(sesid, tableid, SeekGrbit.SeekEQ);
        }

        [TestMethod]
        public void testdosomething()
        {
            dosemthingelse();
        }
        private void dosemthingelse()
        {
            var ret = DoSomething((session, tableid, columnidMessage, columnidMetaData, autoincColumn) =>
                {
                    using (var update = new Update(session, tableid, JET_prep.Insert))
                    {
                        var message = "Hi this is the message";
                        var metadata = "Hi this is the Metadata";

                        Api.SetColumn(session, tableid, columnidMessage, message, Encoding.Unicode);
                        Api.SetColumn(session, tableid, columnidMetaData, metadata, Encoding.Unicode);

                        int? autoinc = Api.RetrieveColumnAsInt32(
                            session,
                            tableid,
                            autoincColumn,
                            RetrieveColumnGrbit.RetrieveCopy);
                        update.Save();
                        Assert.IsTrue(autoinc != null || autoinc != 0);
                        return autoinc;
                    }
                });
        }
        private static int? DoSomething(Func<Session, JET_TABLEID, JET_COLUMNID, JET_COLUMNID, JET_COLUMNID, int?> func)
        {
            using (var instance = new Instance("MyInstanceName"))
            {
                instance.Parameters.CircularLog = true;
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetAttachDatabase(session, DatabaseName, AttachDatabaseGrbit.None);
                    Api.JetOpenDatabase(session, DatabaseName, null, out dbid, OpenDatabaseGrbit.None);

                    JET_TABLEID tableid;

                    if (Api.TryOpenTable(session, dbid, "messages", OpenTableGrbit.None, out tableid))
                    {
                        IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, tableid);
                        JET_COLUMNID autoincColumn = columnids["id"];
                        JET_COLUMNID columnidMessage = columnids["message"];
                        JET_COLUMNID columnidMetaData = columnids["metadata"];
                        using (var transaction = new Transaction(session))
                        {
                            var result = func.Invoke(session, tableid, columnidMessage, columnidMetaData, autoincColumn);

                            transaction.Commit(CommitTransactionGrbit.LazyFlush);
                            return result;
                        }                       
                    }
                    else
                    {
                        throw new Exception("Messages Table does not exist in database");
                    }
                }
            }
        }
    }

}
