using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestUtils
{
    public class TestHelper
    {
        public static AutoResetEvent autoEvent;
        public static AutoResetEvent SubscriberRan;
        public static AutoResetEvent Subscriber1Ran;
        public static AutoResetEvent Subscriber2Ran;
        public static AutoResetEvent Subscriber3Ran;
        public static AutoResetEvent UpdateMessageStoreEvent;
        public static AutoResetEvent SubscriberGroupCompletedEvent;

        public static MessageAdder<T> AddAMessage<T>()
        {
            string name = CleanupName(typeof(T).ToString());
            T message = (T)Activator.CreateInstance(typeof(T));
            var messageAdder = new MessageAdder<T>(message, name);
            return messageAdder;
        }

        public static string CleanupName(string dirtyname)
        {
            return dirtyname.ToString().Replace("{", string.Empty).Replace("}", string.Empty).Replace("_", string.Empty).Replace(".", string.Empty);
        }

        public static MessageBuilder<T> BuildAMessage<T>(T m)
        {
            string name = CleanupName(typeof(T).ToString());
            return new MessageBuilder<T>(m, name);
        }

        public static MessageBuilder<T> BuildAMessage<T>()
        {
            string name = CleanupName(typeof(T).ToString());
            T message = (T)Activator.CreateInstance(typeof(T));
            var messageAdder = new MessageBuilder<T>(message, name);
            return messageAdder;
        }
    }
}
