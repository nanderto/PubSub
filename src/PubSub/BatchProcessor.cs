namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Timers;

    public static class BatchProcessor<T>
    {
        private static bool hasStarted = true;

        private static bool isConfigured = false;

        private static Timer timer = new Timer(10000);
        
        private static bool processRunning = false;

        private static IPublishSubscribeChannel<T> publishSubscribeChannel;

        ////private static Dictionary<string, Tuple<string, Type, TimeSpan>> subscriberInfos = new Dictionary<string, Tuple<string, Type, TimeSpan>>();

        ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        ////static BatchProcessor()
        ////{

        ////}

        public static bool HasStarted
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

        public static bool IsConfigured 
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
            ////subscriberInfos = publishSubscribeChannel.SubscriberInfos;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Start();
            IsConfigured = true;
            HasStarted = true;
        }

        public static void Halt()
        {
            timer.Stop();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to swallow all messages to allow queue processing to continue")]
        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                Trace.WriteLine("About to check if process is still running");
                Counter.Increment(7);
                if (!processRunning)
                {
                    var sw = Stopwatch.StartNew();
                    Counter.Increment(8);
                    Trace.WriteLine("Starting new process");
                    processRunning = true;
                    ProcessBatch();
                    CheckProcessingStatus();
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

        private static void CheckProcessingStatus()
        {
            ////PublishSubscribeChannel.c;
        }

        private static void ProcessBatch()
        {
            publishSubscribeChannel.ProcessBatch();
        }
    }
}
