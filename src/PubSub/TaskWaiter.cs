namespace Phantom.PubSub
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class TaskWaiter
    {
        public CountdownEvent countdownEvent = new CountdownEvent(1);

        private bool _doneAdding = false;

        private ConcurrentQueue<Task> faultedConcurrentQueue = new ConcurrentQueue<Task>(); 

        public void Add(Task task)
        {
            if (task == null) throw new ArgumentNullException("task");
            countdownEvent.AddCount();
            task.ContinueWith(ct =>
            {
                if (ct.Exception != null)
                {
                    faultedConcurrentQueue.Enqueue(ct);
                }
                else
                {
                   
                    countdownEvent.Signal();
                }
            });
        }

        public void Wait()
        {
            if (!_doneAdding) { _doneAdding = true; countdownEvent.Signal(); }
            countdownEvent.Wait();
            if (!faultedConcurrentQueue.IsEmpty) Task.WaitAll(faultedConcurrentQueue.ToArray()); 
        }
    }
}
