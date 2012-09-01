using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phantom.PubSub;
using Entities;

namespace BusinessLogic
{
    /// <summary>
    /// this is a concrete implemantation of the ServiceAdaptor. It is of type "user"
    /// this is the code that kicks everthing off 
    /// </summary>
    public class UserManagerServiceAdaptor : ServiceAdaptor<User>
    {

        private User User{ get; set;}

        /// <summary>
        /// Create the queue provider and the pubsubchannel to put messages to and get them from, and the Filter to wire it up to the adaptor.
        /// other filters could also be added here. In a MVC application this would be created normally in the controllsr
        /// We could new it up or expect that it is created by the dependency rezolver
        /// </summary>
        public UserManagerServiceAdaptor()
        {
            //Once you are commited to using an agaptor you expect at least 1 filter
            var queueProvider = new MsmqQueueProvider<User>() as IQueueProvider<User>;
            IPublishSubscribeChannel<User> PubSubChannel = new PublishSubscribeChannel<User>(queueProvider) as IPublishSubscribeChannel<User>;
            PubSubChannel.AddSubscriberType(typeof(TestSubscriber2<User>));
            this.Register(new PublishMessageFilter<User>(PubSubChannel));
        }

        /// <summary>
        /// THis constructor can be used by unit tests to isolate the use 
        /// 
        /// </summary>
        /// <param name="MsmqQueueProvider">A specialised Queue Provider in this case it is Poly carbonite</param>
        /// <param name="PubSubChannel">A Pubsubchannel of a particular type</param>
        public UserManagerServiceAdaptor(IQueueProvider<User> QueueProvider, IPublishSubscribeChannel<User> PubSubChannel)
        {
            if (QueueProvider == null)
            {
                //provide defalut 
                QueueProvider = new MsmqQueueProvider<User>() as IQueueProvider<User>;
            }
            
            if (PubSubChannel == null)
            {
                PubSubChannel = new PublishSubscribeChannel<User>(QueueProvider) as IPublishSubscribeChannel<User>;
            }

            this.Register(new PublishMessageFilter<User>(PubSubChannel));
        }

        //public UserManagerServiceAdaptor(IQueueProvider<User> QueueProvider, IPublishSubscribeChannel<User> PubSubChannel)
        //{
        //    if (QueueProvider == null)
        //    {
        //        QueueProvider = new QueueProvider<User>() as IQueueProvider<User>;
        //    }
            
        //    if (PubSubChannel == null)
        //    {
        //        PubSubChannel = new UserPublishSubscribeChannel(QueueProvider) as IPublishSubscribeChannel<User>;
        //    }

        //    this.Register(new PublishMessageFilter<User>(PubSubChannel));
        //}

        public void Update(User umToUpdate)
        {
            this.User = umToUpdate;
            this.Execute(umToUpdate);
            DummyData.users.Update(umToUpdate);
        }

        public void Create(User umToUpdate)
        {
            this.User = umToUpdate;
            this.Execute(umToUpdate);
            DummyData.users.Create(umToUpdate);

        }

        public void Remove(string usrName)
        {
            //this.User = umToUpdate;
            //this.Execute(umToUpdate);
            DummyData.users.Remove(usrName);
        }

        public User GetUser(string uid)
        {
            User usrMdl = null;
            //foreach (UserModel um in _usrList)
            //    if (um.UserName == uid)
            //        usrMdl = um;
            return usrMdl;
        }    
    }
}
