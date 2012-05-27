using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantom.PubSub
{
    public class ServiceAdaptor<T> : IFilterChain<T>
    {

        private IFilter<T> root;
        public void Execute(T input)
        {
            if (this.root != null) root.Execute(input);
        }

        public IFilterChain<T> Register(IFilter<T> filter)
        {
            if (this.root == null) this.root = filter;
            else this.root.Register(filter);
            return this;
        }

    }
}
