//-----------------------------------------------------------------------
// <copyright file="IPublishSubscribeChannel.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface IPublishSubscribeChannel. Public interface for all Publish subscribe Channels.
    /// Should always use this interface.
    /// </summary>
    /// <typeparam name="T">Type which this PublishSubscribeChannel is specialized</typeparam>
    public interface IPublishSubscribeChannel<T>
    {
        /// <summary>
        /// Gets the count. Number of messages in channel storage
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        int Count { get; }

        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <returns>Returns <see cref="SubscribersCollection{T}"/></returns>
        SubscribersCollection<T> GetSubscriptions();

        /// <summary>
        /// Processes messages as a batch.
        /// </summary>
        void ProcessBatch();

        /// <summary>
        /// Publishes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void PublishMessage(T message);

        /// <summary>
        /// Adds the type of the subscriber. This method is used to add subscribers via a fluent interface.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns <see cref="ISubscriberInfo{T}"/></returns>
        ISubscriberInfo<T> AddSubscriberType(Type type);
    }
}
