//-----------------------------------------------------------------------
// <copyright file="BatchProcessor.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//----------------------------------------------------------------------
        
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Class BatchProcessor
    /// </summary>
    /// <typeparam name="T">Type to which this batch processor is specialized</typeparam>
    internal static class BatchProcessor<T>
    {
        /// <summary>
        /// Indicates if batch processor has started running
        /// </summary>
        private static bool hasStarted = true;

        /// <summary>
        /// Indicates if Batch processor has been configured correctly
        /// </summary>
        private static bool isConfigured = false;

        /// <summary>
        /// Timer that fires batch processing event. Batch processor fires every 10 seconds and is ot customizable
        /// </summary>
        private static Timer timer = new Timer(OnTimerEvent, null, 10000, 10000);

        /// <summary>
        /// Indicates if the process for this specialized object T is running or not
        /// </summary>
        private static bool processRunning = false;

        /// <summary>
        /// Channel that is used to process the batch
        /// </summary>
        private static IPublishSubscribeChannel<T> publishSubscribeChannel;

        /// <summary>
        /// Gets a value indicating whether this instance has started.
        /// </summary>
        /// <value><c>true</c> if this instance has started; otherwise, <c>false</c>.</value>
        internal static bool HasStarted
        {
            get
            {
                return hasStarted;
            }

            private set
            {
                hasStarted = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is configured.
        /// </summary>
        /// <value><c>true</c> if this instance is configured; otherwise, <c>false</c>.</value>
        internal static bool IsConfigured 
        { 
            get
            {
                return isConfigured;
            } 

            private set
            {
                isConfigured = value;
            }
        }

        /// <summary>
        /// Configures the object with start up data, and starts the timer running.
        /// </summary>
        /// <param name="publishSubscribeChannel">Must be a fully configured publish Subscribe channel, as this channel is used to process the messages.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Need Member to be type specific")]
        public static void ConfigureWithPubSubChannel(IPublishSubscribeChannel<T> publishSubscribeChannel)
        {
            BatchProcessor<T>.publishSubscribeChannel = publishSubscribeChannel;
            IsConfigured = true;
            HasStarted = true;
        }

        /// <summary>
        /// Halts this instance of the batch processing.
        /// </summary>
        public static void Halt()
        {
            timer.Change(0, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Processes the batch.
        /// </summary>
        private static void ProcessBatch()
        {
            publishSubscribeChannel.ProcessBatch();
        }

        /// <summary>
        /// Called when [timer event]. THe process will check and see if the previous process is still running if it is it will not restart the batch processing
        /// </summary>
        /// <param name="state">The state.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions will disapear without catching and logging")]
        private static void OnTimerEvent(object state)
        {
            try
            {
                Trace.WriteLine("ProcessBatch Should fire every 10 seconds: " + DateTime.Now.ToString() + " In Batch Processor About to check if process is still running");
                Counter.Increment(7);
                if (!processRunning)
                {
                    var sw = Stopwatch.StartNew();
                    Counter.Increment(8);
                    Trace.WriteLine("Starting new process");
                    processRunning = true;
                    ProcessBatch();
                    processRunning = false;
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Batch process ran for {0:#,#} ms", sw.ElapsedMilliseconds));
                }
                else
                {
                    Trace.WriteLine("Yes it is");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}
