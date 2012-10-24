//-----------------------------------------------------------------------
// <copyright file="IStoreProvider.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface IStoreProvider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreProvider<T>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Configures the store.
        /// </summary>
        /// <param name="storeName">Name of the store.</param>
        /// <param name="queueTransactionOption">The queue transaction option.</param>
        /// <returns><c>true</c> if store id configured, <c>false</c> otherwise</returns>
        bool ConfigureStore(string storeName, StoreTransactionOption storeTransactionOption);

        /// <summary>
        /// Removes from storage.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <returns><c>true</c> if message is removed from store, <c>false</c> otherwise</returns>
        bool RemoveFromStorage(string messageId);

        /// <summary>
        /// Processes the store as batch.
        /// </summary>
        /// <param name="messageHandlingInitiated">The message handling initiated.</param>
        void ProcessStoreAsBatch(Func<MessagePacket<T>, string, bool> messageHandlingInitiated);

        /// <summary>
        /// Checks its still in the store.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        bool CheckItsStillInTheStore(string messageId);

        /// <summary>
        /// Puts the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>System.String.</returns>
        string PutMessage(MessagePacket<T> message);
    }
}
