using Microsoft.Isam.Esent.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Phantom.PubSub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsentTests
{
    class TestHelper
    {
        private const string TableName = "messages";


        public static void CreateDatabase(string database)
        {
            using (var instance = new Instance("createdatabasetest"))
            {
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetCreateDatabase(session, database, null, out dbid, CreateDatabaseGrbit.OverwriteExisting);
                    using (var transaction = new Transaction(session))
                    {
                        // A newly created table is opened exclusively. This is necessary to add
                        // a primary index to the table (a primary index can only be added if the table
                        // is empty and opened exclusively). Columns and indexes can be added to a 
                        // table which is opened normally.
                        // The other way to create a table is to use JetCreateTableColumnIndex to
                        // add all columns and indexes with one call.
                        JET_TABLEID tableid;
                        Api.JetCreateTable(session, dbid, TableName, 16, 100, out tableid);
                        CreateColumnsAndIndexes(session, tableid);
                        Api.JetCloseTable(session, tableid);

                        // Lazily commit the internalTransaction. Normally committing a internalTransaction forces the
                        // associated log records to be flushed to disk, so the commit has to wait for
                        // the I/O to complete. Using the LazyFlush option means that the log records
                        // are kept in memory and will be flushed later. This will preserve internalTransaction
                        // atomicity (all operations in the internalTransaction will either happen or be rolled
                        // back) but will not preserve durability (a crash after the commit call may
                        // result in the internalTransaction updates being lost). Lazy internalTransaction commits are
                        // considerably faster though, as they don't have to wait for an I/O.
                        transaction.Commit(CommitTransactionGrbit.LazyFlush);
                    }


                    using (var transaction = new Transaction(session))
                    {
                        JET_TABLEID tableid;
                        Api.JetCreateTable(session, dbid, "subscribermetadata", 16, 100, out tableid);
                        CreateColumnsAndIndexesForSubscriberMetadata(session, tableid);
                        Api.JetCloseTable(session, tableid);
                        transaction.Commit(CommitTransactionGrbit.LazyFlush);
                    }
                }
            }
        }

        private static void CreateColumnsAndIndexesForSubscriberMetadata(JET_SESID sesid, JET_TABLEID tableid)
        {
            using (var transaction = new Transaction(sesid))
            {
                JET_COLUMNID columnid;

                var columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long,
                    grbit = ColumndefGrbit.ColumnAutoincrement
                };

                Api.JetAddColumn(sesid, tableid, "id", columndef, null, 0, out columnid);

                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long
                };

                Api.JetAddColumn(sesid, tableid, "messageid", columndef, null, 0, out columnid);

                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.LongText,
                    cp = JET_CP.Unicode
                };

                Api.JetAddColumn(sesid, tableid, "name", columndef, null, 0, out columnid);
                //Api.JetAddColumn(sesid, tableid, "id", columndef, null, 0, out columnid);
                Api.JetAddColumn(sesid, tableid, "timetoexpire", columndef, null, 0, out columnid);//in miliseconds

                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Bit
                };

                Api.JetAddColumn(sesid, tableid, "completed", columndef, null, 0, out columnid);
                Api.JetAddColumn(sesid, tableid, "failedortimedout", columndef, null, 0, out columnid);

                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.DateTime
                };

                Api.JetAddColumn(sesid, tableid, "starttime", columndef, null, 0, out columnid);
                Api.JetAddColumn(sesid, tableid, "failedortimeouttime", columndef, null, 0, out columnid);

                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Short
                };

                Api.JetAddColumn(sesid, tableid, "retrycount", columndef, null, 0, out columnid);

                string indexDef = "+id\0\0";
                Api.JetCreateIndex(sesid, tableid, "primary", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);

                indexDef = "+messageid\0\0";
                Api.JetCreateIndex(sesid, tableid, "messageid", CreateIndexGrbit.None, indexDef, indexDef.Length, 100);

                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }
        /// <summary>
        /// Setup the meta-data for the given table.
        /// </summary>
        /// <param name="sesid">The session to use.</param>
        /// <param name="tableid">
        /// The table to add the columns/indexes to. This table must be opened exclusively.
        /// </param>
        private static void CreateColumnsAndIndexes(JET_SESID sesid, JET_TABLEID tableid)
        {
            using (var transaction = new Transaction(sesid))
            {
                JET_COLUMNID columnid;

                // Stock symbol : text column
                var columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.LongText,
                    cp = JET_CP.Unicode
                };

                Api.JetAddColumn(sesid, tableid, "message", columndef, null, 0, out columnid);

                // Name of the company : text column
                Api.JetAddColumn(sesid, tableid, "metadata", columndef, null, 0, out columnid);

               


                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long,

                    // Be careful with ColumndefGrbit.ColumnNotNULL. Older versions of ESENT
                    // (e.g. Windows XP) do not support this grbit for tagged or variable columns
                    // (JET_coltyp.Text, JET_coltyp.LongText, JET_coltyp.Binary, JET_coltyp.LongBinary)
                    grbit = ColumndefGrbit.ColumnAutoincrement
                };

                Api.JetAddColumn(sesid, tableid, "id", columndef, null, 0, out columnid);

                string indexDef = "+id\0\0";
                Api.JetCreateIndex(sesid, tableid, "primary", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);

                // Number of shares owned (this column may be null) : 32-bit integer
                //columndef.grbit = ColumndefGrbit.None;
                //Api.JetAddColumn(sesid, tableid, "shares_owned", columndef, null, 0, out columnid);

                // Now add indexes. An index consists of several index segments (see
                // EsentVersion.Capabilities.ColumnsKeyMost to determine the maximum number of
                // segments). Each segment consists of a sort direction ('+' for ascending,
                // '-' for descending), a column name, and a '\0' separator. The index definition
                // must end with "\0\0". The count of characters should include all terminators.

                // The primary index is the stock symbol. The primary index is always unique.
                //string indexDef = "+symbol\0\0";
                //Api.JetCreateIndex(sesid, tableid, "primary", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);

                // An index on the company name.
                //indexDef = "+name\0+symbol\0\0";
                //Api.JetCreateIndex(sesid, tableid, "name", CreateIndexGrbit.IndexUnique, indexDef, indexDef.Length, 100);

                // An index on the price. This index is not unique.
                //indexDef = "+price\0\0";
                //Api.JetCreateIndex(sesid, tableid, "price", CreateIndexGrbit.None, indexDef, indexDef.Length, 100);

                // Create 2 indexes that contain either companies where we own shares or companies
                // where we don't own shares. To do this we make the indexes conditional on the
                // 'shares_owned' column being null or non-null. When the 'shares_owned' column
                // is null entries will only appear in the 'noshares' index. When the 'shares_owned'
                // column is non-null (i.e. it has a value) entries will only appear in the 'shares'
                // index. Here we don't index the 'shares_owned' column (that would be valid too),
                // we just use it to determine membership in the index.
            //    const string IndexKey = "+name\0\0";
            //    JET_INDEXCREATE[] indexcreates = new[]
            //{
            //    new JET_INDEXCREATE
            //    {
            //        szIndexName = "shares",
            //        szKey = IndexKey,
            //        cbKey = IndexKey.Length,
            //        rgconditionalcolumn = new[]
            //        {
            //            new JET_CONDITIONALCOLUMN
            //            {
            //                szColumnName = "shares_owned",
            //                grbit = ConditionalColumnGrbit.ColumnMustBeNonNull
            //            }
            //        },
            //        cConditionalColumn = 1
            //    },
            //};

                //// This is important: only create one index at a time with JetCreateIndex2!
                //// The API takes an array of JET_INDEXCREATE objects, but if more than one
                //// index is passed in then the API operates in batch mode, which requires
                //// the caller NOT be in a internalTransaction.
                //Api.JetCreateIndex2(sesid, tableid, indexcreates, indexcreates.Length);

                //// Now the first index has been created we change the name and invert the
                //// condition and create a second index.
                //indexcreates[0].szIndexName = "noshares";
                //indexcreates[0].rgconditionalcolumn[0].grbit = ConditionalColumnGrbit.ColumnMustBeNull;
                //Api.JetCreateIndex2(sesid, tableid, indexcreates, indexcreates.Length);

                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

        public static string CleanupName(string dirtyname)
        {
            return dirtyname.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);
        }

        public static void VerifyDatabase(string databaseName)
        {
            using (var instance = new Instance("MyInstanceName"))
            {
                instance.Parameters.CircularLog = true;
                instance.Init();
                var TableName = "messages";

                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetAttachDatabase(session, databaseName, AttachDatabaseGrbit.None);
                    Api.JetOpenDatabase(session, databaseName, null, out dbid, OpenDatabaseGrbit.None);
                    using (var table = new Table(session, dbid, TableName, OpenTableGrbit.None))
                    {
                        IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, table);
                        JET_COLUMNID columnidMessage = columnids["message"];
                        JET_COLUMNID columnidMetaData = columnids["metadata"];
                        JET_COLUMNID columnidId = columnids["id"];

                        Assert.IsInstanceOfType(columnidId, typeof(JET_COLUMNID));
                        Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                        Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                    }

                    TableName = "subscribermetadata";

                    using (var table = new Table(session, dbid, TableName, OpenTableGrbit.None))
                    {
                        IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, table);
                        JET_COLUMNID columnidMessage = columnids["name"];
                        JET_COLUMNID columnidMessageid = columnids["messageid"];
                        JET_COLUMNID columnidId = columnids["id"];
                        JET_COLUMNID columnidTimetoexpire = columnids["timetoexpire"];

                        Assert.IsInstanceOfType(columnidId, typeof(JET_COLUMNID));
                        Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                        Assert.IsInstanceOfType(columnidMessageid, typeof(JET_COLUMNID));
                        Assert.IsInstanceOfType(columnidTimetoexpire, typeof(JET_COLUMNID));
                    }
                }
            }
        }


        public static List<string> DumpByIndex(JET_SESID sesid, JET_TABLEID tableid, string index, IDictionary<string, JET_COLUMNID> columnids)
        {
            Api.JetSetCurrentIndex(sesid, tableid, index);
            return GetAllRecords(sesid, tableid, columnids);
        }
        private static List<string> GetAllRecords(JET_SESID sesid, JET_TABLEID tableid, IDictionary<string, JET_COLUMNID> columnids)
        {
            List<string> results = null;
            if (Api.TryMoveFirst(sesid, tableid))
            {
                results = GetRecordsToEnd(sesid, tableid, columnids);
            }
            return results;
        }
        private static List<string> GetRecordsToEnd(JET_SESID sesid, JET_TABLEID tableid, IDictionary<string, JET_COLUMNID> columnids)
        {
            List<string> results = new List<string>();
            do
            {
                results.Add(GetOneRow(sesid, tableid, columnids));
            }
            while (Api.TryMoveNext(sesid, tableid));
            return results;
        }

        private static string GetOneRow(JET_SESID sesid, JET_TABLEID tableid, IDictionary<string, JET_COLUMNID> columnids)
        {
            JET_COLUMNID columnidMessage = columnids["message"];
            JET_COLUMNID columnidMetaData = columnids["metadata"];

            string message = Api.RetrieveColumnAsString(sesid, tableid, columnidMessage);
            string Metadata = Api.RetrieveColumnAsString(sesid, tableid, columnidMetaData);
            return message;
        }

        public static MessagePacket<Dummy> GetMessagePacket(Dummy u)
        {
            var message = new MessagePacket<Dummy>(u, GetMetadatas());
            return message;
        }

        public static void Add4MessagesToStore()
        {
            var store = new EsentStoreProvider<Dummy>();
            var result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName", Id = 13 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName1", Id = 14 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName2", Id = 15 }));
            result = store.PutMessage(TestHelper.GetMessagePacket(new Dummy() { Name = "THeDummysName3", Id = 16 }));
            
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        public static bool DoesDatabaseExist(string databaseName)
        {
            bool result = false;

            if (File.Exists(databaseName))
            {
                result = true;
            }
            return result;
        }

        public static List<ISubscriberMetadata> GetMetadatas()
        {
            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = "John",
                TimeToExpire = new TimeSpan(0, 1, 0),
                RetryCount = 0
            };

            var subscribermetadata2 = new SubscriberMetadata()
            {
                Name = "Joe",
                TimeToExpire = new TimeSpan(0, 1, 0),
                RetryCount = 23
            };

            List<ISubscriberMetadata> metadatas = new List<ISubscriberMetadata>();

            metadatas.Add(subscribermetadata1);
            metadatas.Add(subscribermetadata2);
            return metadatas;
        }

        public static MessageBuilder<T> BuildAMessage<T>(T m)
        {
            string name = CleanupName(typeof(T).ToString());
            return new MessageBuilder<T>(m, name);
        }

        public static MessageBuilder<T> BuildAMessage<T>()
        {
            string name = CleanupName(typeof(T).ToString());
            T message = (T)Activator.CreateInstance(typeof(T));
            var messageAdder = new MessageBuilder<T>(message, name);
            return messageAdder;
        }
    }

    public class MessageBuilder<T>
    {
        public MessagePacket<T> messagePacket;
        private string QueueName;
        private T message;
        public List<ISubscriberMetadata> metadatalist;

        public MessageBuilder(T message, string queueName)
        {
            this.message = message;
            this.QueueName = queueName;
        }

        public MessageBuilder<T> WithSubscriberMetadataFor(Type SubscriberType, bool failedOrTimedOut, bool completed)
        {
            if (metadatalist == null)
            {
                metadatalist = new List<ISubscriberMetadata>();
            }

            var r = new Random();
            var subscribermetadata1 = new SubscriberMetadata()
            {
                Name = SubscriberType.Name,// TestHelper.CleanupName(SubscriberType.ToString()),
                FailedOrTimedOut = failedOrTimedOut,
                Completed = completed,
                //TimeToExpire = timeToExpire,
                StartTime = DateTime.Now,
                Id = r.Next().ToString(),
                RetryCount = r.Next(1,9)
            };

            metadatalist.Add(subscribermetadata1);

            if (messagePacket == null)
            {
                messagePacket = new MessagePacket<T>(message, metadatalist);
            }
            return this;
        }

        public MessagePacket<T> GetMessage()
        {
            Random r = new Random();

            messagePacket.MessageId = r.Next();
            return messagePacket;
        }

        public List<ISubscriberMetadata> GetMetadataList()
        {
            return metadatalist;
        }
    }
}
