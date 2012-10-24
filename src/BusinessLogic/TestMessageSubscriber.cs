using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phantom.PubSub;
using Entities;
using System.Threading;

namespace BusinessLogic
{
    public class TestMessageSubscriber : Subscriber<Message>, ISubscriber<Message>
    {
        public override TimeSpan DefaultTimeToExpire
        {
            get
            {
                return new TimeSpan(0, 0, 20);
            }
        }

        public TestMessageSubscriber(MessageService service)
        {
            this.DataService = service;
        }

        private MessageService DataService;

        public override bool Process(Message message)
        {
            if (this.DataService.SaveMessage((Message)message) > 0)
                return true;
            else
                return false;
        }

        public override System.Threading.Tasks.Task<bool> ProcessAsync(Message input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class TestMessageSubscriber2 : Subscriber<Message>
    {
        public TestMessageSubscriber2(MessageService2 service)
        {
            this.DataService = service;
        }

        private MessageService2 DataService;

        public override bool Process(Message message)
        {
            if (this.DataService.SaveMessage((Message)message) > 0)
                return true;
            else
                return false;
        }

        public override System.Threading.Tasks.Task<bool> ProcessAsync(Message input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class TestMessageSubscriber3 : Subscriber<Message>, ISubscriber<Message>
    {
        public TestMessageSubscriber3(MessageService3 service)
        {
            this.DataService = service;
        }

        private MessageService3 DataService;

        public override bool Process(Message message)
        {
            var m = (Message)message;
            m.MessageID = MessageId;
            m.SubscriptionID = this.Id;

            if (this.DataService.SaveMessage(m) > 0)
                return true;
            else
                return false;
        }

        public override System.Threading.Tasks.Task<bool> ProcessAsync(Message input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class TestMessageSubscriber4 : Subscriber<Message>, ISubscriber<Message>
    {
        public TestMessageSubscriber4(MessageService4 service)
        {
            this.DataService = service;
        }

        private MessageService4 DataService;

        public override bool Process(Message message)
        {
            var m = (Message)message;
            m.MessageID = MessageId;
            m.SubscriptionID = this.Id;

            if (this.DataService.SaveMessage(m) > 0)
                return true;
            else
                return false;
        }

        public override System.Threading.Tasks.Task<bool> ProcessAsync(Message input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class TestMessageSubscriber5 : Subscriber<Message>, ISubscriber<Message>
    {
        public override TimeSpan DefaultTimeToExpire
        {
            get
            {
                return new TimeSpan(0, 0, 20);
            }
        }

        public TestMessageSubscriber5(MessageService5 service)
        {
            this.DataService = service;
        }

        private MessageService5 DataService;
        public override bool Process(Message message)
        {
            var m = (Message)message;
            m.MessageID = MessageId;
            m.SubscriptionID = this.Id;

            if (this.DataService.SaveMessage(m) > 0)
                return true;
            else
                return false;
        }


        public override System.Threading.Tasks.Task<bool> ProcessAsync(Message input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }



    public class TestMessageSubscriberUserType : Subscriber<User>, ISubscriber<User>
    {
        public TestMessageSubscriberUserType()
        {

        }
        public TestMessageSubscriberUserType(MessageService service)
        {
            this.DataService = service;
        }

        private MessageService DataService;

        public override bool Process(User message)
        {
            //if (this.DataService.SaveMessage((Message)message) > 0)
            return true;
            //else
            //    return false;
        }


        public override System.Threading.Tasks.Task<bool> ProcessAsync(User input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class TestMessageSubscriberUserType2 : Subscriber<User>, ISubscriber<User>
    {
        public TestMessageSubscriberUserType2()
        {

        }
        public TestMessageSubscriberUserType2(MessageService2 service)
        {
            this.DataService = service;
        }

        private MessageService2 DataService;

        public override bool Process(User message)
        {
            //if (this.DataService.SaveMessage((Message)message) > 0)
            return true;
            //else
            //    return false;
        }

        public override System.Threading.Tasks.Task<bool> ProcessAsync(User input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class TestMessageSubscriberUserType3 : Subscriber<User>, ISubscriber<User>
    {
        public TestMessageSubscriberUserType3()
        {

        }
        public TestMessageSubscriberUserType3(MessageService3 service)
        {
            this.DataService = service;
        }

        private MessageService3 DataService;

        public override bool Process(User message)
        {
            //var m = (Message)message;
            //m.MessageID = MessageId;
            //m.SubscriptionID = SubscriptionId;

            //if (this.DataService.SaveMessage(m) > 0)
            return true;
            //else
            //    return false;
        }


        public override System.Threading.Tasks.Task<bool> ProcessAsync(User input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
