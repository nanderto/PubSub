//-----------------------------------------------------------------------
// <copyright file="SubscriberInfo.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains data describing the Subscribers associated with the specific PubSubChannel
    /// Contains methods for constructing this data and attaching to PubSubChannel
    /// </summary>
    /// <typeparam name="T">The type that this component handles</typeparam>
    public class SubscriberInfo<T> : ISubscriberInfo<T>
    {
        private Type subscriberType = null;

        private IPublishSubscribeChannel<T> publishSubscribeChannel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberInfo{T}" /> class.
        /// </summary>
        /// <param name="subscriberType">Type of the subscriber.</param>
        /// <param name="publishSubscribeChannel">The publish subscribe channel.</param>
        public SubscriberInfo(Type subscriberType, IPublishSubscribeChannel<T> publishSubscribeChannel)
        {
            this.subscriberType = subscriberType;
            this.publishSubscribeChannel = publishSubscribeChannel;
        }

        public IPublishSubscribeChannel<T> WithTimeToExpire(TimeSpan timeSpan)
        {
            return ((PublishSubscribeChannel<T>)this.publishSubscribeChannel).AddSubscriberInfo(Tuple.Create(this.subscriberType.Name, this.subscriberType, timeSpan));
        }
    }
}
