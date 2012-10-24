//-----------------------------------------------------------------------
// <copyright file="SubscriberMetadataConverter.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Class for JSON deserializer to deserialized into the correct object
    /// </summary>
    public class SubscriberMetadataConverter : CustomCreationConverter<ISubscriberMetadata>
    {
        public override ISubscriberMetadata Create(Type objectType)
        {
            return new SubscriberMetadata();
        }
    }
}
