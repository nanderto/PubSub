//-----------------------------------------------------------------------
// <copyright file="IFilterChain.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Interface IFilterChain
    /// </summary>
    /// <typeparam name="T">Type that this interface is specialized to</typeparam>
    public interface IFilterChain<T>
    {
        /// <summary>
        /// Executes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        void Execute(T input);

        /// <summary>
        /// Registers the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Returns a object of type IFilterChain. effectively returns itself, an object that contains the chain of filters that will act on the passed in object</returns>
        IFilterChain<T> Register(IFilter<T> filter);
    }
}
