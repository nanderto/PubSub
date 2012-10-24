// ***********************************************************************
// Assembly         : Phantom.PubSub
// Author           : Noel
// Created          : 09-04-2012
//
// Last Modified By : Noel
// Last Modified On : 09-04-2012
// ***********************************************************************
// <copyright file="ISubscriberMetadata.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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

        /// <summary>
        /// Determines whether this instance can process.
        /// </summary>
        /// <returns><c>true</c> if this instance can process; otherwise, <c>false</c>.</returns>
        bool CanProcess();
    }
}
