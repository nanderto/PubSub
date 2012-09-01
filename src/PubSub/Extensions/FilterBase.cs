namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public abstract class FilterBase<T> : IFilter<T>
    {
        private IFilter<T> next;
      
        public T Execute(T input)
        {
            T val = this.Process(input);
            if (this.next != null) val = this.next.Execute(val);
            return val;
        }

        public void Register(IFilter<T> filter)
        {
            if (this.next == null)
            {
                this.next = filter;
            }
            else
            {
                this.next.Register(filter);
            }
        }

        protected abstract T Process(T input);
    }
}
