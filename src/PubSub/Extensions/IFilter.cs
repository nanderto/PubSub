namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IFilter<T>
    {
         T Execute(T input);

         void Register(IFilter<T> filter);
    }
}
