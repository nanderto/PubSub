//-----------------------------------------------------------------------
// <copyright file="ServiceAdaptor.cs" company="The Phantom Coder">
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
    /// Class to inherit your service adaptors from
    /// </summary>
    /// <typeparam name="T">The type to specialise the adaptor to</typeparam>
    public class ServiceAdaptor<T> : IFilterChain<T>
    {
        /// <summary>
        /// Root Filter
        /// </summary>
        private IFilter<T> root;

        /// <summary>
        /// Executes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void Execute(T input)
        {
            if (this.root != null)
            {
                this.root.Execute(input);
            }
        }

        /// <summary>
        /// Registers the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Returns IFilterChain{T}.</returns>
        public IFilterChain<T> Register(IFilter<T> filter)
        {
            if (this.root == null)
            {
                this.root = filter;
            }
            else
            {
                this.root.Register(filter);
            }

            return this;
        }
    }
}
