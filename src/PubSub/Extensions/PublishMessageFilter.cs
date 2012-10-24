//-----------------------------------------------------------------------
// <copyright file="PublishMessageFilter.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Messaging;
    using System.Text;

    /// <summary>
    /// Class PublishMessageFilter This filter will publish your object via the PubSubChannel to any Subscribers listed. It is specialised to the class that 
    /// you want to publish
    /// </summary>
    /// <typeparam name="T">Specilalization for this Filter</typeparam>
    public class PublishMessageFilter<T> : FilterBase<T>
    {
        /// <summary>
        /// PubSubChannel to publish input to
        /// </summary>
        private IPublishSubscribeChannel<T> publishSubscribeChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishMessageFilter{T}" /> class.
        /// </summary>
        /// <param name="publishSubscribeChannel">The publish subscribe channel.</param>
        public PublishMessageFilter(IPublishSubscribeChannel<T> publishSubscribeChannel)
        {
            this.publishSubscribeChannel = publishSubscribeChannel;
        }

        /// <summary>
        /// Processes the specified input. Calls the PubSubChannel and publishes the message
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns the input after publishing</returns>
        protected override T Process(T input)
        {
            this.publishSubscribeChannel.PublishMessage(input);
            return input;
        }
    }
}
