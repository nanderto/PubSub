using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Phantom.PubSub;
using System.Threading.Tasks;

namespace TestUtils
{
    /// <summary>
    /// does nothing returns as fast as possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpeedySubscriber<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            return true;
        }

        public override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {
            return DoSomethingAsync();
        }

        private async Task<bool> DoSomethingAsync()
        {
            Counter.Increment(0);
            if (TestHelper.autoEvent != null)
            {
                TestHelper.autoEvent.Set();
            }

            if (TestUtils.TestHelper.Subscriber1Ran != null)
            {
                TestUtils.TestHelper.Subscriber1Ran.Set();
            }
            return true;
        }
    }

    /// <summary>
    /// does nothing returns as fast as possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpeedySubscriber2<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            return true;
        }

        public override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {
           
            return DoSomethingAsync();
        }

        private async Task<bool> DoSomethingAsync()
        {
            Counter.Increment(1);
            //if (TestHelper.autoEvent != null)
            //{
            //    TestHelper.autoEvent.Set();
            //}

            if (TestUtils.TestHelper.Subscriber2Ran != null)
            {
                TestUtils.TestHelper.Subscriber2Ran.Set();
            }
            return true;
        }
    }

    /// <summary>
    /// does nothing returns as fast as possible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpeedySubscriberRandomExceptions<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            return true;
        }

        public override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {

            return DoSomethingAsync();
        }

        private async Task<bool> DoSomethingAsync()
        {
            if (TestUtils.TestHelper.Subscriber3Ran != null)
            {
                TestUtils.TestHelper.Subscriber3Ran.Set();
            }

            Counter.Increment(2);
            Random r = new Random();
            if (r.Next(80, 100) > 95)
            {
                Counter.Increment(3);
                throw new ApplicationException("the request threw an exception, for testing purposes");
            }
            return true;
        }
    }

    public class SpeedySubscriberGuaranteedExceptions<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            return true;
        }

        public override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {

            return DoSomethingAsync();
        }

        private async Task<bool> DoSomethingAsync()
        {
            if (TestUtils.TestHelper.Subscriber3Ran != null)
            {
                TestUtils.TestHelper.Subscriber3Ran.Set();
            }
            Counter.Increment(3); 
            throw new ApplicationException("the request threw an exception, for testing purposes");
        }
    }

    public class SpeedySubscriberGuaranteedCancelled<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            return true;
        }

        public override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {
            Counter.Increment(3);
            if (TestUtils.TestHelper.Subscriber3Ran != null)
            {
                TestUtils.TestHelper.Subscriber3Ran.Set();
            }
            cancellationToken.ThrowIfCancellationRequested();
            return DoSomethingAsync();
        }

        private async Task<bool> DoSomethingAsync()
        {           
            throw new ApplicationException("the request threw an exception, for testing purposes");
        }
    }

    class MyClass : Subscriber<LocalDummy>
    {

        public override bool Process(LocalDummy input)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ProcessAsync(LocalDummy input, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }

    class LocalDummy
    {
        public void method()
        {
            var x = new MyClass();
        }
    }
}