using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantom.PubSub
{
    public interface IFilterChain<T>
    {
        void Execute (T input);
        IFilterChain<T> Register(IFilter<T> filter);
    }

    //public interface IFilterChain2<T>
    //{
    //    T Execute(T input);
    //    IFilterChain2<T> Register(IFilter<T> filter);
    //}
}
