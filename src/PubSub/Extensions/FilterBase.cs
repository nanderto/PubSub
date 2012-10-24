//-----------------------------------------------------------------------
// <copyright file="FilterBase.cs" company="The Phantom Coder">
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
    /// Class FilterBase
    /// </summary>
    /// <typeparam name="T">Type for which you wish to specialize this class. </typeparam>
    public abstract class FilterBase<T> : IFilter<T>
    {
        /// <summary>
        /// Next filter to call
        /// </summary>
        private IFilter<T> next;

        /// <summary>
        /// Executes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The Type that this class is specialized for</returns>
        public T Execute(T input)
        {
            T val = this.Process(input);
            if (this.next != null)
            {
                val = this.next.Execute(val);
            }

            return val;
        }

        /// <summary>
        /// Registers the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
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

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The Type that this class is specialized for</returns>
        protected abstract T Process(T input);
    }
}
