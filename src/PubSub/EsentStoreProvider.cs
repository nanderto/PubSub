//-----------------------------------------------------------------------
// <copyright file="EsentStoreProvider.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Isam.Esent.Interop;

    public class EsentStoreProvider<T> : IStoreProvider<T>
    {
        private static readonly object SyncLock = new object();
        private static volatile bool isStoreConfigured = false;
        private string longStoreName = string.Empty;

        public EsentStoreProvider()
        {
            this.Name = CleanupName(typeof(T).ToString());
            this.longStoreName = this.Name + ".edb";

            if (!isStoreConfigured)
            {
                lock (SyncLock)
                {
                    isStoreConfigured = this.ConfigureStore(this.longStoreName, StoreTransactionOption.SupportTransactions);
                }
            }
        }

        public string Name { get; set; }

        public static bool DoesDatabaseExist(string databaseName)
        {
            bool result = false;

            using (var instance = new Instance("DoesDatabaseExistInstance"))
            {
                instance.Parameters.CircularLog = true;
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    try
                    {
                        Api.JetAttachDatabase(session, databaseName, AttachDatabaseGrbit.None);
                        Api.JetOpenDatabase(session, databaseName, null, out dbid, OpenDatabaseGrbit.None);
                        ////JET_TABLEID tableid;
                        result = true;
                        ////does not seem to be working 
                        ////if (Api.TryOpenTable(session, dbid, "message", OpenTableGrbit.None, out tableid))
                        ////{
                        ////    IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, tableid);
                        ////    JET_COLUMNID columnidMessage = columnids["message"];
                        ////    JET_COLUMNID columnidMetaData = columnids["metadata"];
                        ////    result = true;
                        ////    //Assert.IsInstanceOfType(columnidMessage, typeof(JET_COLUMNID));
                        ////    //Assert.IsInstanceOfType(columnidMetaData, typeof(JET_COLUMNID));
                        ////    //AddMessage(session, tableid, ref columnidMessage, ref columnidMetaData);
                        ////}
                    }
                    catch (EsentFileNotFoundException)
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        public static void CreateDatabase(string database)
        {
            var tableName = "messages";

            using (var instance = new Instance("createdatabase"))
            {
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetCreateDatabase(session, database, null, out dbid, CreateDatabaseGrbit.OverwriteExisting);
                    using (var transaction = new Transaction(session))
                    {
                        JET_TABLEID tableid;
                        Api.JetCreateTable(session, dbid, tableName, 16, 100, out tableid);
                        CreateColumnsAndIndexes(session, tableid);
                        Api.JetCloseTable(session, tableid);
                        transaction.Commit(CommitTransactionGrbit.LazyFlush);
                    }
                }
            }
        }

        public bool ConfigureStore(string storeName, StoreTransactionOption storeTransactionOption)
        {
            if (!DoesDatabaseExist(storeName))
            {
                CreateDatabase(storeName);
            }

            return true;
        }

        public bool RemoveFromStorage(string messageId)
        {
            throw new NotImplementedException();
        }

        public void ProcessStoreAsBatch(Func<MessagePacket<T>, string, bool> messageHandlingInitiated)
        {
            throw new NotImplementedException();
        }

        public bool CheckItsStillInTheStore(string messageId)
        {
            throw new NotImplementedException();
        }

        public string PutMessage(MessagePacket<T> message)
        {
            throw new NotImplementedException();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is spelled correctly")]
        public List<MessagePacket<T>> GetAllMessages()
        {
            using (var instance = new Instance("GetAllMessagesInstance"))
            {
                instance.Parameters.CircularLog = true;
                instance.Init();
                using (var session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetAttachDatabase(session, this.longStoreName, AttachDatabaseGrbit.None);
                    Api.JetOpenDatabase(session, this.longStoreName, null, out dbid, OpenDatabaseGrbit.None);

                    JET_TABLEID tableid;

                    if (Api.TryOpenTable(session, dbid, "messages", OpenTableGrbit.None, out tableid))
                    {
                        IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(session, tableid);

                        return GetAllRecords(session, tableid, null, columnids);
                    }
                    else
                    {
                        throw new ApplicationException("Messages Table does not exist in database");
                    }
                }
            }
        }

        private static string CleanupName(string dirtyname)
        {
            return dirtyname.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);
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

                var columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.LongText,
                    cp = JET_CP.Unicode
                };

                Api.JetAddColumn(sesid, tableid, "message", columndef, null, 0, out columnid);
                Api.JetAddColumn(sesid, tableid, "metadata", columndef, null, 0, out columnid);

                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long,
                    grbit = ColumndefGrbit.ColumnAutoincrement
                };

                Api.JetAddColumn(sesid, tableid, "id", columndef, null, 0, out columnid);

                string indexDef = "+id\0\0";
                Api.JetCreateIndex(sesid, tableid, "primary", CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);

                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

        private static List<MessagePacket<T>> GetAllRecords(JET_SESID sesid, JET_TABLEID tableid, string index, IDictionary<string, JET_COLUMNID> columnids)
        {
            Api.JetSetCurrentIndex(sesid, tableid, index);
            return GetAllRecords(sesid, tableid, columnids);
        }

        private static List<MessagePacket<T>> GetAllRecords(JET_SESID sesid, JET_TABLEID tableid, IDictionary<string, JET_COLUMNID> columnids)
        {
            List<MessagePacket<T>> results = null;
            if (Api.TryMoveFirst(sesid, tableid))
            {
                results = GetRecordsToEnd(sesid, tableid, columnids);
            }

            return results;
        }

        private static List<MessagePacket<T>> GetRecordsToEnd(JET_SESID sesid, JET_TABLEID tableid, IDictionary<string, JET_COLUMNID> columnids)
        {
            List<MessagePacket<T>> results = new List<MessagePacket<T>>();
            do
            {
                results.Add(GetRow(sesid, tableid, columnids));
            }
            while (Api.TryMoveNext(sesid, tableid));
            return results;
        }

        private static MessagePacket<T> GetRow(JET_SESID sesid, JET_TABLEID tableid, IDictionary<string, JET_COLUMNID> columnids)
        {
            JET_COLUMNID columnidId = columnids["id"]; 
            JET_COLUMNID columnidMessage = columnids["message"];
            JET_COLUMNID columnidMetaData = columnids["metadata"];
             
            int? id = Api.RetrieveColumnAsInt32(sesid, tableid, columnidId);
            string message = Api.RetrieveColumnAsString(sesid, tableid, columnidMessage);
            string metadata = Api.RetrieveColumnAsString(sesid, tableid, columnidMetaData);
             
            return new MessagePacket<T>(GetBody(message), GetMetadata(metadata))
                {
                    Id = id.ToString()
                };
        }

        private static List<ISubscriberMetadata> GetMetadata(string metadata)
        {
            throw new NotImplementedException();
        }

        private static T GetBody(string message)
        {
            throw new NotImplementedException();
        }
    }
}
