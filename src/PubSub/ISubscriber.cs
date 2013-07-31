//-----------------------------------------------------------------------
// <copyright file="ISubscriber.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//----------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Phantom.PubSub;

    /// <summary>
    /// Interface ISubscriber
    /// </summary>
    /// <typeparam name="T">The type that this component handles</typeparam>
    public interface ISubscriber<T>
    {
        /// <summary>
        /// Gets or sets the abort count.
        /// </summary>
        /// <value>The abort count.</value>
        int AbortCount { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>The message id.</value>
        string MessageId { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISubscriber`1" /> is aborted.
        /// </summary>
        /// <value><c>true</c> if aborted; otherwise, <c>false</c>.</value>
        bool Aborted { get; set; }

        /// <summary>
        /// Gets or sets the time to expire.
        /// </summary>
        /// <value>The time to expire.</value>
        TimeSpan TimeToExpire { get; set; }

        /// <summary>
        /// Gets the default time to expire.
        /// </summary>
        /// <value>The default time to expire.</value>
        TimeSpan DefaultTimeToExpire { get; }
        
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the aborted time.
        /// </summary>
        /// <value>The aborted time.</value>
        DateTime AbortedTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [finished processing].
        /// </summary>
        /// <value><c>true</c> if [finished processing]; otherwise, <c>false</c>.</value>
        bool FinishedProcessing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [started processing].
        /// </summary>
        /// <value><c>true</c> if [started processing]; otherwise, <c>false</c>.</value>
        bool StartedProcessing { get; set; }

        /// <summary>
        /// Aborts this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        bool Abort();

        /// <summary>
        /// Runs the async.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task of Bool.</returns>
        Task<bool> RunAsync(T message, CancellationToken cancellationToken);
    }
}