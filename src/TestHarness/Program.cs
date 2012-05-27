using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;



namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var q = TestHelper.GetQueue();
            Message M = q.ReceiveById(@"b21493a6-eda6-46f6-a1ea-e19e6aa69518\529159", new TimeSpan(0));
            M.Formatter = new BinaryMessageFormatter();
            string MessageId = M.Id;
            Entities.Message msg = (Entities.Message)M.Body;
        }

        public void test()
        {
            
        }
    }

    public static class TestHelper
    {
        private static MessageQueue msgQ;
        public static void SetUpCleanTestQueue()
        {
            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUser"))
                MessageQueue.Delete(@".\private$\EntitiesUser");
            MessageQueue.Create(@".\private$\EntitiesUser", true);

            //Clean up if exists
            if (MessageQueue.Exists(@".\private$\EntitiesUserPoisonMessages"))
                MessageQueue.Delete(@".\private$\EntitiesUserPoisonMessages");
            MessageQueue.Create(@".\private$\EntitiesUserPoisonMessages", true);
        }

        public static MessageQueue GetQueue()
        {
            MessageQueue.Create(@".\private$\EntitiesUser", true);
            msgQ = new MessageQueue(@".\private$\EntitiesUser");
            return msgQ;
        }
    }
}
