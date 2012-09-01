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
    public class TestSubscriber<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            // int index = 1;
            Counter.Increment(1);
            Counter.Increment(0);
            return true;
        }
    }

    public class TestSubscriberXXX<Message> : Subscriber<Message>
    {
        public override bool Process(Message input)
        {
            Counter.Increment(2);
            Counter.Increment(0);
            return true;
        }
    }

    public class TestSubscriber2<T> : Subscriber<T>
    {
        #region ISubscriber Members

        public override bool Process(T input)
        {
            Counter.Increment(3);
            Counter.Increment(0);
           // System.Diagnostics.Debug.WriteLine("Writing stuff: {0}", "");
            return true;
        }

        #endregion
    }

    public class TestSubscriber3<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            System.Diagnostics.Debug.WriteLine("Writing stuff: {0}", "");
            return true;
        }


    }
}