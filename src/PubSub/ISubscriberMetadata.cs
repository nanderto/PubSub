//-----------------------------------------------------------------------
// <copyright file="ISubscriberMetadata.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//----------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Interface ISubscriberMetadata
    /// </summary>
    public interface ISubscriberMetadata
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>The message id.</value>
        int MessageId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time to expire.
        /// </summary>
        /// <value>The time to expire.</value>
        TimeSpan TimeToExpire { get; set; }

        int RetryCount { get; set; }

        bool Completed { get; set; }

        bool FailedOrTimedOut { get; set; }

        DateTime FailedOrTimedOutTime { get; set; }

        /// <summary>
        /// Determines whether this instance can process.
        /// </summary>
        /// <returns><c>true</c> if this instance can process; otherwise, <c>false</c>.</returns>
        bool CanProcess();

        bool CanProcess(ICurrentTimeProvider currentTime);
    }
}
