//-----------------------------------------------------------------------
// <copyright file="Subscriber.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//----------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
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
        
        public string Name { get; set; }

        public string Id { get; set; }

        public virtual string MessageId { get; set; }

        public DateTime AbortedTime { get; set; }

        public TimeSpan TimeToExpire { get; set; }

        public virtual TimeSpan DefaultTimeToExpire 
        {
            get
            {
                return new TimeSpan(0, 0, 100);
            }
        }

        public DateTime StartTime { get; set; }

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

        public int AbortCount
        {
            get
            {
                return this.abortCount;
            }

            set
            {
                this.abortCount = value;
            }
        }

        /// <summary>
        /// Abstract method that must be overridden to provide your processing.
        /// This method should not be called directly in your code. I should probably
        /// figure out ow to make it safer from poor coding. 
        /// </summary>
        /// <param name="input">Message to be input</param>
        /// <returns>True on success</returns>
        public abstract bool Process(T input);

        public abstract Task<bool> ProcessAsync(T input, CancellationToken cancellationToken);

        /// <summary>
        /// Sets the status of the subscriber to the aborted state. Records the time it occured increments the abort count and sets bool values for started adn finish processing to false.
        /// Used when timed out or an exception is thrown
        /// </summary>
        /// <returns>Always returns true need to change to void</returns>
        public bool Abort()
        {
            Counter.Increment(15);
            this.Aborted = true;
            this.AbortedTime = DateTime.Now;
            this.abortCount = ++this.abortCount;
            this.StartedProcessing = false;
            this.FinishedProcessing = false;
            Trace.WriteLine("The task with the MessagId: " + this.MessageId + " and the SUbscriberID: " + this.Id + " is being Aborted for the: " + this.abortCount + " Time ");
            return true;
        }
        
        public async Task<bool> RunAsync(T message, CancellationToken cancellationToken)
        {
            ////Trace.WriteLine("RunAsync About to start: MessageId: " + this.MessageId + " SubscriberID: " + this.Id);
                    
            cancellationToken.ThrowIfCancellationRequested();
            this.PreProcess();
            cancellationToken.ThrowIfCancellationRequested();
            var result = await this.ProcessAsync(message, cancellationToken);
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
            return true;
        }
    }
}
