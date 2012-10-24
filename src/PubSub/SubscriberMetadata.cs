//-----------------------------------------------------------------------
// <copyright file="SubscriberMetadata.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class SubscriberMetadata. Contains data about the specific message and subscriber combination, including information like start time.
    /// Also has functions that act on metadata. this information is persisted with the message when the message is saved to disk
    /// </summary>
    [Serializable]
    public class SubscriberMetadata : ISubscriberMetadata
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time to expire.
        /// </summary>
        /// <value>The time to expire.</value>
        public TimeSpan TimeToExpire { get; set; }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>The retry count.</value>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the failed or timed out time.
        /// </summary>
        /// <value>The failed or timed out time.</value>
        public DateTime FailedOrTimedOutTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [failed or timed out].
        /// </summary>
        /// <value><c>true</c> if [failed or timed out]; otherwise, <c>false</c>.</value>
        public bool FailedOrTimedOut { get; set; }

        /// <summary>
        /// Determines whether this instance can process. Applies a exponential increase to restarting, each retry results in a 2^number of retrys longer time between retrying.
        /// </summary>
        /// <returns><c>true</c> if this instance can process; otherwise, <c>false</c>.</returns>
        public bool CanProcess()
        {
            if (HasExpired(this))
            {
                double time = Math.Pow(2, this.RetryCount);
                TimeSpan ts = new TimeSpan(0, Convert.ToInt32(time), 0);

                DateTime nextstart = this.FailedOrTimedOutTime + ts;

                if (DateTime.Compare(DateTime.Now, nextstart) < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false; // all else fails return false to ensure it is ot restarted unnessarily
            //// it can always get picked up in the next loop
        }

        /// <summary>
        /// Determines whether the specified subscriber has expired by interogating the meta data.
        /// </summary>
        /// <param name="subscriberMetaData">The subscriber meta data.</param>
        /// <returns><c>true</c> if the specified subscriber meta data has expired; otherwise, <c>false</c>.</returns>
        private static bool HasExpired(ISubscriberMetadata subscriberMetaData)
        {
            var nextstart = subscriberMetaData.StartTime + subscriberMetaData.TimeToExpire;
            var now = DateTime.Now;
            if (DateTime.Compare(now, nextstart) > 0)
            {
                return true;
            }

            return false;
        }
    }
}
