namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IFilterChain<T>
    {
        void Execute(T input);

        IFilterChain<T> Register(IFilter<T> filter);
    }
}
