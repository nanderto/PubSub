//-----------------------------------------------------------------------
// <copyright file="ICurrentTimeProvider.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//----------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Interface for the Current Time Provider allows replacing with fake in testing
    /// </summary>
    public interface ICurrentTimeProvider
    {
        DateTime Now { get; }
    }
}
