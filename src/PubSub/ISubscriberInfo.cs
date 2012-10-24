//-----------------------------------------------------------------------
// <copyright file="ISubscriberInfo.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Interface for the SubscriberInfo
    /// </summary>
    /// <typeparam name="T">The type that this component handles</typeparam>
    public interface ISubscriberInfo<T>
    {
        IPublishSubscribeChannel<T> WithTimeToExpire(TimeSpan timeSpan);
    }
}
