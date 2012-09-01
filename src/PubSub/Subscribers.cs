namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class creates a list of ISubscribers of the generic type.
    /// It is a collection class that is used to hold a list of subscribers for a specific message.
    /// It is also used to determine if all members have completed processing and to call out to removue if they have.
    /// </summary>
    /// <typeparam name="T">Type that this subscription is set up for</typeparam>
    public class Subscribers<T> : List<ISubscriber<T>>
    {
        private object syncLock = new object();
        private bool allSubscribersDone = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Need Func of generic type")]
        public bool IfAllSubscribersCompletedLockAndRemove(Func<Subscribers<T>, bool> removeFromQueue)
        {
            if (removeFromQueue == null) throw new ArgumentNullException("removeFromQueue");

            List<ISubscriber<T>> completedSubscribers = null;
            int completedCount = 0;

            lock (this.syncLock)
            {
                if (!this.allSubscribersDone)
                {
                    completedSubscribers = this.FindAll(s => s.FinishedProcessing != true);
                    completedCount = completedSubscribers.Count();

                    if (completedCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        this.allSubscribersDone = true;
                        return removeFromQueue(this);
                    }
                }

                return false;
            }
        }
    }
}
