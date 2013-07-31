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
        /// Initializes a new instance of the <see cref="SubscriberMetadata" /> class.
        /// </summary>
        public SubscriberMetadata()
        {
            this.FailedOrTimedOut = false;
            this.Completed = false;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>The message id.</value>
        public int MessageId { get; set; }

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

        public bool Completed { get; set; }

        /// <summary>
        /// Determines whether the specified subscriber has expired by interogating the meta data.
        /// </summary>
        /// <param name="subscriberMetadata">The subscriber meta data.</param>
        /// <param name="currentTime">The current time.</param>
        /// <returns>
        ///   <c>true</c> if the specified subscriber meta data has expired; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Argument Null Exception</exception>
        public static bool HasExpired(ISubscriberMetadata subscriberMetadata, ICurrentTimeProvider currentTime)
        {
            if (currentTime == null)
            {
                throw new ArgumentNullException("currentTime");
            }

            if (subscriberMetadata == null)
            {
                throw new ArgumentNullException("subscriberMetadata");
            }

            var nextstart = subscriberMetadata.StartTime + subscriberMetadata.TimeToExpire;
            if (DateTime.Compare(currentTime.Now, nextstart) > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether this instance can process. Applies a exponential increase to restarting, each retry results in a 2^number of retrys longer time between retrying.
        /// </summary>
        /// <returns><c>true</c> if this instance can process; otherwise, <c>false</c>.</returns>
        public bool CanProcess()
        {
            return this.CanProcess(new DefaultCurrentTimeProvider());
        }

        /// <summary>
        /// Determines whether this instance can process the specified current time.
        /// Applies a exponential increase to restarting, each retry results in a 2^number of retrys longer time between retrying.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        /// <returns>
        ///   <c>true</c> if this instance can process the specified current time; otherwise, <c>false</c>.
        /// </returns>
        public bool CanProcess(ICurrentTimeProvider currentTime)
        {
            if (this.Completed)
            {
                return false;
            }

            if (HasExpired(this, currentTime))
            {
                if (currentTime == null)
                {
                    throw new ArgumentNullException("currentTime");
                }

                double time = Math.Pow(2, this.RetryCount);
                TimeSpan ts = new TimeSpan(0, Convert.ToInt32(time), 0);

                DateTime nextstart;
                if (this.FailedOrTimedOutTime == null || this.FailedOrTimedOutTime == DateTime.MinValue)
                {
                    ////If it has expired but not yet had the failed or timed out set then we are processing a record from the database that has not 
                    ////yet saved the time that it expired(or failed). we could Ignore it, but if something happend in the update of the failed or timeed out 
                    ////time then we would never run this subscriber again. Instead lets restart it after the correct amount of time has elapsed 
                    nextstart = this.StartTime + ts;
                    ////of course this could result in a subscriber that just lkeeps restarting over and over and failing (or timing out over and over)
                    ////need to add updating the store with retry count 
                }
                else 
                {
                    nextstart = this.FailedOrTimedOutTime + ts;
                }
                
                nextstart = this.FailedOrTimedOutTime + ts;

                if (DateTime.Compare(currentTime.Now, nextstart) < 0)
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
        /// Default implementation of Current Time provider returns system time
        /// </summary>
        internal class DefaultCurrentTimeProvider : ICurrentTimeProvider
        {
            public DateTime Now
            {
                get { return DateTime.Now; }
            }
        }
    }
}
