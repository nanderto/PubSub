//-----------------------------------------------------------------------
// <copyright file="IQueueProvider.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
using System;
using System.Collections.Generic;

    public interface IQueueProvider<T>
    {
        string Name { get; set; }
        
        string ConfigureQueue(string queueName, QueueTransactionOption queueTransactionOption);
       
        void SetupWatchQueue(IQueueProvider<T> queueProvider);
        
        bool RemoveFromQueue(string messageId);
        
        void ProcessQueueAsBatch(Func<MessagePacket<T>, string, bool> messageHandlingInitiated);

        bool CheckItsStillInTheQueue(string messageId);

        string PutMessageInTransaction(MessagePacket<T> message);
    }
}
