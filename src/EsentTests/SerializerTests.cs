using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EsentTests
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod, TestCategory("UnitTest")]
        public void CheckSerializer()
        {
            var d = new Dummy();
            d.Id = 23;
            d.Name = "Vernon";
            var mp = TestHelper.GetMessagePacket(d);
            var input = Serializer.Serialize<MessagePacket<Dummy>>(mp);
            var input2 = Serializer.GetSerializedBody<Dummy>(mp);

            Assert.AreEqual(input, input2);
            MessagePacket<Dummy> messagePacket = Serializer.DeserializeMessagePacket<Dummy>(input);

            //MessagePacket<Dummy> mp2 = (MessagePacket<Dummy>)JsonConvert.DeserializeObject<MessagePacket<Dummy>>(input2, new SubscriberMetadataConverter());

            Assert.IsInstanceOfType(messagePacket, typeof(MessagePacket<Dummy>));
            Assert.IsInstanceOfType(messagePacket.Body, typeof(Dummy));
            Assert.IsTrue(messagePacket.SubscriberMetadataList != null);
            Assert.IsTrue(messagePacket.SubscriberMetadataList.Count == 2);
            Assert.IsInstanceOfType(messagePacket.SubscriberMetadataList[0], typeof(ISubscriberMetadata));

            Assert.IsInstanceOfType(messagePacket.SubscriberMetadataList[1], typeof(SubscriberMetadata));
            Assert.IsTrue(messagePacket.SubscriberMetadataList[0].Name == "John");
            Assert.IsTrue(messagePacket.SubscriberMetadataList[1].Name == "Joe");
            Assert.IsTrue(messagePacket.Body.Id == 23);
            Assert.IsTrue(messagePacket.Body.Name.StartsWith("Vernon"));

        }

        [TestMethod, TestCategory("UnitTest")]
        public void CheckSerializer2()
        {
            var d = new Dummy();
            d.Id = 23;
            d.Name = "Vernon";
            var mp = TestHelper.GetMessagePacket(d);

            MessagePacket<Dummy> messagePacket = Serializer.DeserializeMessagePacket<Dummy>(Serializer.GetSerializedBody<Dummy>(mp), Serializer.GetSerializedMetadata(mp.SubscriberMetadataList));
            Assert.IsInstanceOfType(messagePacket, typeof(MessagePacket<Dummy>));
            Assert.IsInstanceOfType(messagePacket.Body, typeof(Dummy));
            Assert.IsTrue(messagePacket.SubscriberMetadataList != null);
            Assert.IsTrue(messagePacket.SubscriberMetadataList.Count == 2);
            Assert.IsInstanceOfType(messagePacket.SubscriberMetadataList[0], typeof(ISubscriberMetadata));

            Assert.IsInstanceOfType(messagePacket.SubscriberMetadataList[1], typeof(SubscriberMetadata));
            Assert.IsTrue(messagePacket.SubscriberMetadataList[0].Name == "John");
            Assert.IsTrue(messagePacket.SubscriberMetadataList[1].Name == "Joe");
            Assert.IsTrue(messagePacket.Body.Id == 23);
            Assert.IsTrue(messagePacket.Body.Name.StartsWith("Vernon"));

        }
    }
}
