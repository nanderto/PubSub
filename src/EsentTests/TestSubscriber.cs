using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Phantom.PubSub;
using System.Threading.Tasks;

namespace EsentTests
{
    public class EsentTestSubscriber<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            return true;
        }

        public async override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {
            //Thread.Sleep(10);
            cancellationToken.ThrowIfCancellationRequested();
                Counter.Increment(1);
                Counter.Increment(0);
                
                //Trace.WriteLine("yes I ran 1 I am not set up to run async");
            //var result = new Task<bool>(() => true);
                return true;
            }
        }

    public class EsentTestSubscriberXXX<T> : Subscriber<T>
    {

        public override TimeSpan DefaultTimeToExpire
        {
            get
            {
                return new TimeSpan(0, 0, 20);
            }
        }

        public override bool Process(T input)
        {
            return true;
        }

        public async override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {
            Random r = new Random();
            //Thread.Sleep(r.Next(80, 110));
           // await Task.Delay(r.Next(80, 110));
            cancellationToken.ThrowIfCancellationRequested();
            var result = await DoSomethingAsync();
            //Trace.WriteLine("afte doing something Asyn happens, this happens");
            
            return result;
        }

        private async Task<bool> DoSomethingAsync()
        {
           // Thread.Sleep(20);
            Counter.Increment(2);
            Counter.Increment(0);
            //Trace.WriteLine("yes I ranxxxx and I should not have33333333333333");
           // var result = await new Task<bool>(() => true);
            return true;
        }

    }

    public class EsentTestSubscriber2<T> : Subscriber<T>
    {
        #region ISubscriber Members

        public override bool Process(T input)
        {
            Counter.Increment(3);
            Counter.Increment(0);
            ////Thread.Sleep(10);
            Trace.WriteLine("yes I ran3");
           // System.Diagnostics.Debug.WriteLine("Writing stuff: {0}", "");
            return true;
        }

        #endregion

        public async override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {
            Random r = new Random();
            if (r.Next(80, 100) > 95) throw new ApplicationException("the request threw an exception, for testing purposes");
            cancellationToken.ThrowIfCancellationRequested();
                return await DoSomethingAsync();
            }

        private async Task<bool> DoSomethingAsync()
        {
            //Thread.Sleep(20);
            Counter.Increment(3);
            Counter.Increment(0);
            //Trace.WriteLine("yes I ranxxxx and I should not have4444444444444");
           // var result = await new Task<bool>(() => true);
            return true;
        }
    }

    public class EsentTestSubscriber3<T> : Subscriber<T>
    {
        public override bool Process(T input)
        {
            System.Diagnostics.Debug.WriteLine("Writing stuff: {0}", "");
            return true;
        }

        public override Task<bool> ProcessAsync(T input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

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
            Counter.Increment(2);
            Random r = new Random();
            if (r.Next(0, 100) > 95)
            {
                Counter.Increment(3);
                throw new ApplicationException("the request threw an exception, for testing purposes");
            }
            return true;
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