using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantom.PubSub
{
    public  abstract class Filterbase<T> : IFilter<T>
    {
        private IFilter<T> next;
        
        protected abstract T Process(T input);
      
        #region IFilter<T> Members
      
        public T Execute(T input)
        {
            T val = Process(input);
            if( next != null ) val = next.Execute(val);
            return val;
        }

        public void Register(IFilter<T> filter)
        {
            if( next == null ) next = filter;
            else next.Register(filter);
        }
        
        #endregion

    }
}
