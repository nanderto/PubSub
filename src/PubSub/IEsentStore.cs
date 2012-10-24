//-----------------------------------------------------------------------
// <copyright file="IEsentStore.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Isam.Esent.Interop;

    /// <summary>
    /// Interface for the EsentStore
    /// </summary>
    /// <typeparam name="T">THe Type that is being Handeled by this component</typeparam>
    public interface IEsentStore<T>
    {
        int? AddMessageInTransaction(string body, string metadata);
        
        void AddMetadata(int? messageId, List<ISubscriberMetadata> metadatas);
        
        void CloseSession();
        
        void Commit();
        
        void DeleteMessage(string messageId);
        
        void DeleteMetadata(IEnumerable<ISubscriberMetadata> metadatas);
        
        void Dispose();
        
        IEnumerable<MessagePacket<T>> GetAllMessages();
        
        List<ISubscriberMetadata> GetMetadata(int messageId);
        
        ISubscriberMetadata GetMetadata(int messageId, int metadataId);
        
        int GetRecordCount();
        
        Session OpenSession();
        
        Transaction OpenTransaction();
        
        bool PeekForMessage(string messageId);
        
        void Rollback();
        
        void Save();
        
        void UpdateMetadata(ISubscriberMetadata subscriberMetadata);
    }
}
