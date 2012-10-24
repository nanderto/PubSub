//-----------------------------------------------------------------------
// <copyright file="SubscribersCollection.cs" company="The Phantom Coder">
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
    /// This class creates a list of ISubscribers of the generic type.
    /// It is a collection class that is used to hold a list of subscribers for a specific message.
    /// It is also used to determine if all members have completed processing and to call out to removue if they have.
    /// </summary>
    /// <typeparam name="T">Type that this subscription is set up for</typeparam>
    public class SubscribersCollection<T> : List<ISubscriber<T>>
    {
    }
}
