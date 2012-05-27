using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phantom.PubSub;
using Entities;

namespace BusinessLogic
{
    public class TestMessageSubscriber : Subscriber<Message>, ISubscriber<Message>
    {
        public TestMessageSubscriber(MessageService service)
        {
            this.DataService = service;
        }

        private MessageService DataService;

        public override bool Process(Message message, string MessageId, string SubscriptionId, IMessageStatus<Message> SubScriptionStatus, List<IMessageStatus<Message>> TrackIfStarted)
        {
            if (this.DataService.SaveMessage((Message)message) > 0)
                return true;
            else
                return false;
        }
    }

    public class TestMessageSubscriber2 : Subscriber<Message>, ISubscriber<Message>
    {
        public TestMessageSubscriber2(MessageService2 service)
        {
            this.DataService = service;
        }

        private MessageService2 DataService;

        public override bool Process(Message message, string MessageId, string SubscriptionId, IMessageStatus<Message> SubScriptionStatus, List<IMessageStatus<Message>> TrackIfStarted)
        {
            if (this.DataService.SaveMessage((Message)message) > 0)
                return true;
            else
                return false;
        }
    }

    public class TestMessageSubscriber3 : Subscriber<Message>, ISubscriber<Message>
    {
        public TestMessageSubscriber3(MessageService3 service)
        {
            this.DataService = service;
        }

        private MessageService3 DataService;

        public override bool Process(Message message, string MessageId, string SubscriptionId, IMessageStatus<Message> SubScriptionStatus, List<IMessageStatus<Message>> TrackIfStarted)
        {
            var m = (Message)message;
            m.MessageID = MessageId;
            m.SubscriptionID = SubscriptionId;
            
            if (this.DataService.SaveMessage(m) > 0)
                return true;
            else
                return false;
        }
    }

    public class TestMessageSubscriber4 : Subscriber<Message>, ISubscriber<Message>
    {
        public TestMessageSubscriber4(MessageService4 service)
        {
            this.DataService = service;
        }

        private MessageService4 DataService;

        public override bool Process(Message message, string MessageId, string SubscriptionId, IMessageStatus<Message> SubScriptionStatus, List<IMessageStatus<Message>> TrackIfStarted)
        {
            var m = (Message)message;
            m.MessageID = MessageId;
            m.SubscriptionID = SubscriptionId;

            if (this.DataService.SaveMessage(m) > 0)
                return true;
            else
                return false;
        }
    }

    public class TestMessageSubscriber5 : Subscriber<Message>, ISubscriber<Message>
    {
        public TestMessageSubscriber5(MessageService5 service)
        {
            this.DataService = service;
        }

        private MessageService5 DataService;

        public override bool Process(Message message, string MessageId, string SubscriptionId, IMessageStatus<Message> SubScriptionStatus, List<IMessageStatus<Message>> TrackIfStarted)
        {
            var m = (Message)message;
            m.MessageID = MessageId;
            m.SubscriptionID = SubscriptionId;

            if (this.DataService.SaveMessage(m) > 0)
                return true;
            else
                return false;
        }
    }



    public class TestMessageSubscriberUserType : Subscriber<User>, ISubscriber<User>
    {
        public TestMessageSubscriberUserType(MessageService service)
        {
            this.DataService = service;
        }

        private MessageService DataService;

        public override bool Process(User message, string MessageId, string SubscriptionId, IMessageStatus<User> SubScriptionStatus, List<IMessageStatus<User>> TrackIfStarted)
        {
            //if (this.DataService.SaveMessage((Message)message) > 0)
                return true;
            //else
            //    return false;
        }

    }

    public class TestMessageSubscriberUserType2 : Subscriber<User>, ISubscriber<User>
    {
        public TestMessageSubscriberUserType2(MessageService2 service)
        {
            this.DataService = service;
        }

        private MessageService2 DataService;

        public override bool Process(User message, string MessageId, string SubscriptionId, IMessageStatus<User> SubScriptionStatus, List<IMessageStatus<User>> TrackIfStarted)
        {
            //if (this.DataService.SaveMessage((Message)message) > 0)
                return true;
            //else
            //    return false;
        }
    }

    public class TestMessageSubscriberUserType3 : Subscriber<User>, ISubscriber<User>
    {
        public TestMessageSubscriberUserType3(MessageService3 service)
        {
            this.DataService = service;
        }

        private MessageService3 DataService;

        public override bool Process(User message, string MessageId, string SubscriptionId, IMessageStatus<User> SubScriptionStatus, List<IMessageStatus<User>> TrackIfStarted)
        {
            //var m = (Message)message;
            //m.MessageID = MessageId;
            //m.SubscriptionID = SubscriptionId;

            //if (this.DataService.SaveMessage(m) > 0)
                return true;
            //else
            //    return false;
        }

    }


    public class TestMessageMessageStatusTracker<T> : IMessageStatus<T>
    {
        public bool FinishedProcessing { get; set; }

        public bool StartedProcessing { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string MessageId { get; set; }

        public TimeSpan TimeToExpire { get; set; }
    }

    public class TestMessageMessageStatusTracker2<T> : IMessageStatus<T>
    {
        public bool FinishedProcessing { get; set; }

        public bool StartedProcessing { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string MessageId { get; set; }

        public TimeSpan TimeToExpire { get; set; }
    }

    public class TestMessageMessageStatusTracker3<T> : IMessageStatus<T>
    {
        public bool FinishedProcessing { get; set; }

        public bool StartedProcessing { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string MessageId { get; set; }

        public TimeSpan TimeToExpire { get; set; }
    }
    //public class TestMessageSubscriberGenericType<T> : Subscriber<T>, IMessageStatus<T>
    //{
    //    public TestMessageSubscriberGenericType(MessageService service)
    //    {
    //        this.DataService = service;
    //    }

    //    public bool StartedProcessing { get; set; }
    //    public bool FinishedProcessing { get; set; }
    //    private MessageService DataService;

    //    public override bool Process(T message, string MessageId, string SubscriptionId, IMessageStatus<T> SubScriptionStatus, List<IMessageStatus<T>> TrackIfStarted)
    //    {
    //        //test subscriber does nothing
    //        //if (this.DataService.SaveMessage((Message)message) > 0)
    //        return true;
    //        //else
    //        //    return false;
    //    }
    //}

    //public class TestMessageSubscriberGenericType2<T> : Subscriber<T>, IMessageStatus<T>
    //{
    //    public TestMessageSubscriberGenericType2(MessageService2 service)
    //    {
    //        this.DataService = service;
    //    }
    //    public bool FinishedProcessing { get; set; }
    //    private MessageService2 DataService;

    //    public override bool Process(T message, string MessageId, string SubscriptionId, IMessageStatus<T> SubScriptionStatus, List<IMessageStatus<T>> TrackIfStarted)
    //    {
    //        //if (this.DataService.SaveMessage((Message)message) > 0)
    //        return true;
    //        //else
    //        //    return false;
    //    }


    //    #region IMessageStatus<T> Members


    //    public bool StartedProcessing { get; set; }


    //    #endregion
    //}

    //public class TestMessageSubscriberGenericType3<T> : Subscriber<T>, IMessageStatus<T>
    //{
    //    public TestMessageSubscriberGenericType3(MessageService3 service)
    //    {
    //        this.DataService = service;
    //    }

    //    private MessageService3 DataService;

    //    public override bool Process(T message, string MessageId, string SubscriptionId, IMessageStatus<T> SubScriptionStatus, List<IMessageStatus<T>> TrackIfStarted)
    //    {
    //        //var m = (Message)message;
    //        //m.MessageID = MessageId;
    //        //m.SubscriptionID = SubscriptionId;

    //        //if (this.DataService.SaveMessage(m) > 0)
    //        return true;
    //        //else
    //        //    return false;
    //    }

    //    public bool StartedProcessing { get; set; }
    //    public bool FinishedProcessing { get; set; }
    //}


}
