//-----------------------------------------------------------------------
// <copyright file="Serializer.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//--------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Serializer for serializing the messages being persisted to the Database
    /// </summary>
    public static class Serializer
    {
        public static string Serialize<T>(T input)
        {
            return JsonConvert.SerializeObject(input, Formatting.None);
        }

        public static T Deserializer<T>(string input)
        {
            return (T)JsonConvert.DeserializeObject<T>(input);
        }

        public static string GetSerializedMetadata(List<ISubscriberMetadata> input)
        {
            return JsonConvert.SerializeObject(input, Formatting.None);
        }

        public static string GetSerializedBody<T>(MessagePacket<T> input)
        {
            return JsonConvert.SerializeObject(input, Formatting.None);
        }

        public static MessagePacket<T> DeserializeMessagePacket<T>(string body, string metadata)
        {
            var mp = (MessagePacket<T>)JsonConvert.DeserializeObject<MessagePacket<T>>(body, new SubscriberMetadataConverter());
            mp.ReplaceMetadatas((List<ISubscriberMetadata>)JsonConvert.DeserializeObject<List<ISubscriberMetadata>>(metadata, new SubscriberMetadataConverter()));
            return mp;
        }

        public static MessagePacket<T> DeserializeMessagePacket<T>(string body)
        {
            var mp = (MessagePacket<T>)JsonConvert.DeserializeObject<MessagePacket<T>>(body, new SubscriberMetadataConverter());
            var metadata = JObject.Parse(body);
            IList<JToken> metadataList = metadata["SubscriberMetadataList"].Children().ToList();
            var metadatas = new List<ISubscriberMetadata>();
            foreach (var item in metadataList)
            {
                var ret = JsonConvert.DeserializeObject<ISubscriberMetadata>(item.ToString(), new SubscriberMetadataConverter());
                metadatas.Add(ret);
            }

            mp.ReplaceMetadatas((List<ISubscriberMetadata>)metadatas);
            return mp;
        }
    }
}