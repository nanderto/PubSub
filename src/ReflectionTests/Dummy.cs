using Phantom.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTests
{
    [Serializable]
    public class Dummy
    {
        public string Name { get; set; }
    }

    public class DummySubscriberWithDefault : Subscriber<Dummy>
    {
        public override bool Process(Dummy input)
        {
            throw new NotImplementedException();
        }

        public async override System.Threading.Tasks.Task<bool> ProcessAsync(Dummy input, System.Threading.CancellationToken cancellationToken)
        {
            return await DoSomethingAsync();
        }

        public override TimeSpan DefaultTimeToExpire
        {
            get
            {
                return new TimeSpan(0, 0, 20);
            }
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

    public class DummySubscriberWithOutDefault : Subscriber<Dummy>
    {
        public override bool Process(Dummy input)
        {
            throw new NotImplementedException();
        }

        public async override System.Threading.Tasks.Task<bool> ProcessAsync(Dummy input, System.Threading.CancellationToken cancellationToken)
        {
            return await DoSomethingAsync();
        }

        private async Task<bool> DoSomethingAsync()
        {
            Counter.Increment(3);
            Counter.Increment(0);
            return true;
        }
    }
}
