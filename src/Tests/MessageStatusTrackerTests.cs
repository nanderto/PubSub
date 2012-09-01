using System;
using Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phantom.PubSub;

namespace UnitTests
{
    [TestClass]
    public class MessageStatusTrackerTests
    {
        [TestMethod, TestCategory ("UnitTests")]
        public void IfAllSubscribersCompletedLockandRemoveTest_addonestatusandreturnfalse()
        {
            var sub = new TestSubscriber<User>();
            var subscribers = new Subscribers<User>();
            sub.FinishedProcessing = false;
            subscribers.Add(sub);
            var result = subscribers.IfAllSubscribersCompletedLockAndRemove(removeFromQueue);
            Assert.AreEqual(false, result);
        }

        [TestMethod, TestCategory("UnitTests")]
        public void IfAllSubscribersCompletedLockandRemoveTest_add2statusandreturnfalse()
        {
            var subscribers = new Subscribers<User>();
            var sub = new TestSubscriber<User>();
           // sub.FinishedProcessing = true;
            subscribers.Add(sub);

            var sub2 = new TestSubscriber<User>();
           // sub2.FinishedProcessing = true;
            subscribers.Add(sub2);

            var result = subscribers.IfAllSubscribersCompletedLockAndRemove(removeFromQueue);
            Assert.AreEqual(false, result);
        }

        [TestMethod, TestCategory("UnitTests")]
        public void IfAllSubscribersCompletedLockandRemoveTest_add1statusandreturnTrue()
        {
            var sub = new TestSubscriber<User>();
            var subscribers = new Subscribers<User>();
            sub.FinishedProcessing = true;
            subscribers.Add(sub);
            var result = subscribers.IfAllSubscribersCompletedLockAndRemove(removeFromQueue);
            Assert.AreEqual(true, result);
        }

        [TestMethod, TestCategory("UnitTests")]
        public void IfAllSubscribersCompletedLockandRemoveTest_add2statusandreturnTrue()
        {
            var subscribers = new Subscribers<User>();
            var sub = new TestSubscriber<User>();
            sub.FinishedProcessing = true;
            subscribers.Add(sub);

            var sub2 = new TestSubscriber<User>();
            sub2.FinishedProcessing = true;
            subscribers.Add(sub2);

            var result = subscribers.IfAllSubscribersCompletedLockAndRemove(removeFromQueue);
            Assert.AreEqual(true, result);
        }

        [TestMethod, TestCategory("UnitTests")]
        public void IfAllSubscribersCompletedLockandRemoveTest_add2statusandreturnFalse()
        {
            var subscribers = new Subscribers<User>();
            var sub = new TestSubscriber<User>();
            sub.FinishedProcessing = true;
            subscribers.Add(sub);

            var sub2 = new TestSubscriber<User>();
            sub2.FinishedProcessing = false;
            subscribers.Add(sub2);

            var result = subscribers.IfAllSubscribersCompletedLockAndRemove(removeFromQueue);
            Assert.AreEqual(false, result);
        }

        [TestMethod, TestCategory("UnitTests")]
        public void IfAllSubscribersCompletedLockandRemoveTest_add3statusandreturnFalse()
        {
            var subscribers = new Subscribers<User>();
            var sub = new TestSubscriber<User>();
            sub.FinishedProcessing = true;
            subscribers.Add(sub);

            var sub2 = new TestSubscriber<User>();
            sub2.FinishedProcessing = false;
            subscribers.Add(sub2);

            var sub3 = new TestSubscriber<User>();
            sub3.FinishedProcessing = true;
            subscribers.Add(sub3);

            var result = subscribers.IfAllSubscribersCompletedLockAndRemove(removeFromQueue);
            Assert.AreEqual(false, result);
        }

        [TestMethod, TestCategory("UnitTests")]
        public void IfAllSubscribersCompletedLockandRemoveTest_add3statusandreturnTrue()
        {
            var subscribers = new Subscribers<User>();
            var sub = new TestSubscriber<User>();
            sub.FinishedProcessing = true;
            subscribers.Add(sub);

            var sub2 = new TestSubscriber<User>();
            sub2.FinishedProcessing = true;
            subscribers.Add(sub2);

            var sub3 = new TestSubscriber<User>();
            sub3.FinishedProcessing = true;
            subscribers.Add(sub3);

            var result = subscribers.IfAllSubscribersCompletedLockAndRemove(removeFromQueue);
            Assert.AreEqual(true, result);
        }

        //private bool removeFromQueue(string MessageId, MessageStatusTrackers<User> StartFinishStatus)
        //{
        //    return true;
        //}

        private bool removeFromQueue(Subscribers<User> StartFinishStatus)
        {
            return true;
        }
    }
}
