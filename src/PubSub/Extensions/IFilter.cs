//-----------------------------------------------------------------------
// <copyright file="IFilter.cs" company="The Phantom Coder">
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
    /// Interface IFilter
    /// </summary>
    /// <typeparam name="T">Type which this interface is specialized to</typeparam>
    public interface IFilter<T>
    {
        /// <summary>
        /// Executes the specified input.
        /// </summary>
        /// <param name="input">The input. An instance of the class that this class is specialized to (T)</param>
        /// <returns>Returns the object that was passed in, after completing what ever processing is specified in the filter</returns>
         T Execute(T input);

         /// <summary>
         /// Registers the specified filter.
         /// </summary>
         /// <param name="filter">The filter.</param>
         void Register(IFilter<T> filter);
    }
}
