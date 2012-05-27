using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Phantom.PubSub;


namespace BusinessLogic
{


    public class TestSubscriber<T> : Subscriber<T>, IMessageStatus<T>
    {
        #region ISubscriber Members

        public override bool Process(T input, string MessageId, string SubscriptionId, IMessageStatus<T> SubScriptionStatus, List<IMessageStatus<T>> TrackIfStarted)
        {
            
            //base.Process(input, MessageId, SubscriptionId, SubScriptionStatus, TrackIfStarted);
            int index = 0;
            if (this.Name == "TestSubscriber1`1") index = 1;
            if (this.Name == "TestSubscriber2`1") index = 2;
            Counter.Increment(index);
            LogEntry logEntry = new LogEntry();

            logEntry.Message = "processing: " + SubscriptionId + Environment.NewLine +
                "Subscriber Name: " + this.Name + Environment.NewLine +
                "Subscriber Name: " + this.Name + Environment.NewLine +
                "Counter Index: " + index + " Count: " + Counter.Subscriber(index).ToString() + Environment.NewLine +
               " TotalCount: " + Counter.TotalSubscriberCount().ToString();
            Logger.Write(logEntry);
            
            
            System.Diagnostics.Trace.WriteLine("processing: " + SubscriptionId);
            System.Diagnostics.Trace.WriteLine("Subscriber Name: " + this.Name);
            System.Diagnostics.Trace.WriteLine("Counter Index: " + index + " Count: "+ Counter.Subscriber(index).ToString());
            System.Diagnostics.Trace.WriteLine("Count: " + Counter.TotalSubscriberCount().ToString());
            //PerformanceCounter SubscriberCounter = new PerformanceCounter();
            //SubscriberCounter.CategoryName = "PubSub";
            //SubscriberCounter.CounterName = "Count of subscribers";
            //SubscriberCounter.MachineName = ".";
            //SubscriberCounter.ReadOnly = false;
            //SubscriberCounter.Increment();

           
            return true;
        }

        public override bool Process(T input)
        {          
            System.Diagnostics.Debug.WriteLine("Writing stuff: {0}", "");
            return true;
        }

        #endregion

        #region IMessageStatus<T> Members

        public bool FinishedProcessing { get; set; }
        public bool StartedProcessing  { get; set; }

        #endregion
    }

    public class TestSubscriber2<T> : Subscriber<T>, IMessageStatus<T>
    {
        #region ISubscriber Members

        public override bool Process(T input, string MessageId, string SubscriptionId, IMessageStatus<T> SubScriptionStatus, List<IMessageStatus<T>> TrackIfStarted)
        {

            //base.Process(input, MessageId, SubscriptionId, SubScriptionStatus, TrackIfStarted);
            int index = 0;
            if (this.Name == "Sid") index = 1;
            if (this.Name == "Rube") index = 2;
            Counter.Increment(index);
            LogEntry logEntry = new LogEntry();

            logEntry.Message = "processing: " + SubscriptionId + Environment.NewLine +
                "Subscriber Name: " + this.Name + Environment.NewLine +
                "Subscriber Name: " + this.Name + Environment.NewLine +
                "Counter Index: " + index + " Count: " + Counter.Subscriber(index).ToString() + Environment.NewLine +
               " TotalCount: " + Counter.TotalSubscriberCount().ToString();
            //Logger.Write(logEntry);


            System.Diagnostics.Trace.WriteLine("processing: " + SubscriptionId);
            System.Diagnostics.Trace.WriteLine("Subscriber Name: " + this.Name);
            System.Diagnostics.Trace.WriteLine("Counter Index: " + index + " Count: " + Counter.Subscriber(index).ToString());
            System.Diagnostics.Trace.WriteLine("Count: " + Counter.TotalSubscriberCount().ToString());



            return true;
        }

        public override bool Process(T input)
        {
            System.Diagnostics.Debug.WriteLine("Writing stuff: {0}", "");
            return true;
        }

        #endregion

        #region IMessageStatus<T> Members

        public bool FinishedProcessing { get; set; }
        public bool StartedProcessing { get; set; }

        #endregion
    }
}
