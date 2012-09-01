namespace Phantom.PubSub
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using Phantom.PubSub;

    /// <summary>
    /// The subscriber is the class to inherit from to create your custom subscriptions to the message.
    /// Overwrite the Process method with the custom method you wish. The PubSub Channel will contiue to call 
    /// the Run command until the subscription acknowledges that it has finnished processing.
    /// </summary>
    /// <typeparam name="T">Create a subscriber for your specific type of Message</typeparam>
    public abstract class Subscriber<T> : ISubscriber<T>
    {
        private int abortCount = 0;

        private bool aborted;

        public virtual event OnProcessStartedEventHandler OnProcessStartedEventHandler;

        public virtual event OnProcessCompletedEventHandler OnProcessCompletedEventHandler;

        public string Name { get; set; }

        public string Id { get; set; }

        public string MessageId { get; set; }

        public DateTime AbortedTime { get; set; }

        public TimeSpan TimeToExpire { get; set; }

        public DateTime StartTime { get; set; }

        public Subscribers<T> SubscribersForThisMessage { get; set; }

        public bool StartedProcessing { get; set; }

        public bool FinishedProcessing { get; set; }

        public bool Aborted
        {
            get
            {
                return this.aborted;
            }

            set
            {
                this.aborted = value;
                this.AbortedTime = DateTime.Now;
            }
        }

        public DateTime ExpireTime
        {
            get
            {
                return this.StartTime + this.TimeToExpire;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Abstract method that must be overridden to provide your processing.
        /// This method should not be called directly in your code. I should proably
        /// figure out ow to make it safer from poor coding. 
        /// </summary>
        /// <param name="input">Message to be input</param>
        /// <returns>True on success</returns>
        public abstract bool Process(T input);

        public bool Abort()
        {
            this.Aborted = true;
            this.AbortedTime = DateTime.Now;
            this.abortCount = ++this.abortCount;
            this.StartedProcessing = false;
            this.FinishedProcessing = false;
            return true;
        }
 
        /// <summary>
        /// If the subscriber has been aborted then we can check if it is time to reprocess
        /// </summary>
        /// <returns>True if it meets rules for reprocessing</returns>
        public bool CanProcess()
        {
            if (this.Aborted)
            {             
                // see if it is time for a restart.
                double time = Math.Pow(2, this.abortCount);
                TimeSpan ts = new TimeSpan(0, Convert.ToInt32(time), 0);
                
                DateTime nextstart = this.AbortedTime + ts;

                if (DateTime.Compare(DateTime.Now, nextstart) < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false; // all else fails return false to snsure it is ot restarted unnessarily
            // it can always get picked up in the next loop
        }

        /// <summary>
        /// Call this method to execute your custom process. It will run the PreProcess then the process method and finally Post processing
        /// </summary>
        /// <param name="message">Message being processed</param>
        /// <returns>True on success</returns>
        public bool Run(T message)
        {
            if (this.PreProcess()) 
            {
                // do some thing on false 
            }

            if (this.Process(message))
            {
                // do some thing on false 
            }

            return this.PostProcess();
        }

        /// <summary>
        /// Private method called by Run command, runs after the method Process has being called. Completes Post processing tasks of
        /// setting up the object and raising the OnProcessCompleted event
        /// </summary>
        /// <returns>True on sucess</returns>
        private bool PostProcess()
        {
            this.FinishedProcessing = true;
            this.OnProcessCompletedEventHandler(this, new ProcessCompletedEventArgs(this));
            return true;
        }

        /// <summary>
        /// Private method called by Run command, prior to Process being called. Completes Preprocessing tasks of
        /// setting up the object and raising the OnProcessStarted event
        /// </summary>
        /// <returns>true on sucess</returns>
        private bool PreProcess()
        {
            this.StartedProcessing = true;
            this.StartTime = DateTime.Now;
            this.Aborted = false;
            this.OnProcessStartedEventHandler(this, new ProcessStartedEventArgs(this));
            return true;
        }
    }
}
