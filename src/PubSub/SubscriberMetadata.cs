namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class SubscriberMetadata : ISubscriberMetadata
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan TimeToExpire { get; set; }

        public int RetryCount { get; set; }

        public DateTime FailedOrTimedOutTime { get; set; }

        public bool FailedOrTimedOut { get; set; }

        private static bool HasExpired(ISubscriberMetadata subscriberMetaData)
        {
            var nextstart = subscriberMetaData.StartTime + subscriberMetaData.TimeToExpire;
            if (DateTime.Compare(DateTime.Now, nextstart) > 0)
            {
                return true;
            }
            return false;
        }

        public bool CanProcess()
        {
            if (HasExpired(this))
            {
                //removed this check we shouled only judge wheter to restart based on time because ther is a chanve the flags will not get set correctly
                //if (this.FailedOrTimedOut)
                //{
                    // see if it is time for a restart.
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
                //}
            }
            return false; // all else fails return false to snsure it is ot restarted unnessarily
            // it can always get picked up in the next loop
        }
    }
}
