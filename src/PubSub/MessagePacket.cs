//-----------------------------------------------------------------------
// <copyright file="MessagePacket.cs" company="The Phantom Coder">
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
    /// MessagePacket is a wrapper for the Message it contains metadata about the message, the process and the subscribers 
    /// that is persisted along with the message to the store
    /// </summary>
    /// <typeparam name="T">The type that this component handles</typeparam>
    [Serializable]
    public class MessagePacket<T> 
    {
        private T body = default(T);

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePacket{T}" /> class.
        /// </summary>
        public MessagePacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePacket{T}" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessagePacket(T message)
        {
            this.Body = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePacket{T}" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="subscriberMetadataList">The subscriber metadata list.</param>
        public MessagePacket(T message, List<ISubscriberMetadata> subscriberMetadataList)
        {
            this.Body = message;
            this.SubscriberMetadataList = subscriberMetadataList;
        } 

        public string Id { get; set; }

        public string Name { get; set; }

        public List<ISubscriberMetadata> SubscriberMetadataList { get; private set; }

        public T Body
        {
            get { return this.body; }
            set { this.body = value; }
        }

        public int? MessageId { get; set; }

        public void ReplaceMetadatas(IEnumerable<ISubscriberMetadata> metaDatas)
        {
            if (this.SubscriberMetadataList != null)
            {
                this.SubscriberMetadataList.Clear();
            }
            else
            {
                this.SubscriberMetadataList = new List<ISubscriberMetadata>();
            }

            this.SubscriberMetadataList.AddRange(metaDatas);
        }

        public void AddRange(IEnumerable<ISubscriberMetadata> metaDatas)
        {
            this.SubscriberMetadataList.AddRange(metaDatas);
        }
    }
}
