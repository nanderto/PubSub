
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Phantom.PubSub;

    public class MessageStatus<T> : List<IMessageStatus<T>>
    {
        private object SyncLock = new object();
        private bool AllSubscribersDone = false;
        
        //public new MessageStatus<T> Add(IMessageStatus<T> MessageStatus)
        //{
        //    this.Add(MessageStatus);
        //    return this;
        //}
        
        public bool IfAllSubscribersStartedandCompletedLockandRemove(RemoveMessageFromQueue<T> removeFromQueue, string MessageId)
        {
            List<IMessageStatus<T>> completedSubscribers = null;
            int completedCount = 0;

            ////LogEntry log = new LogEntry();
            ////log.Message = ("IfAllSubscribersStartedandCompletedLockandRemove: " + MessageId + "::this.AllSubscribersDone:" + this.AllSubscribersDone);
            ////Logger.Write(log); 

            lock (this.SyncLock)
            {
                ////log.Message = ("IfAllSubscribersStartedandCompletedLockandRemove after lock: " + MessageId + "::this.AllSubscribersDone:" + this.AllSubscribersDone);
                ////Logger.Write(log); 
                if (!this.AllSubscribersDone)
                {
                    completedSubscribers = this.FindAll(s => s.FinishedProcessing != true);
                    completedCount = completedSubscribers.Count();

                    ////log.Message = ("CompletedCount: " + CompletedCount.ToString() + "::>0 means not all completed returning false " + MessageId);
                    ////Logger.Write(log); 

                    if (completedCount > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("AllSubscribersCompleted: Not yet");
                        return false;
                    }
                    else
                    {
                        // Just removed check on if they have started again 
                        // that should not matter it means that they got restarted some how and are reprocessing but 
                        // if they got finished even once that should be OK.
                        // no need to let processing keep going.
                        ////StartedSubscribers = this.FindAll(s => s.StartedProcessing != true);
                        ////StartedCount = StartedSubscribers.Count();

                        ////log.Message = ("StartedCount: " + StartedCount.ToString() + "::>0 means not all Started returning false " + MessageId);
                        ////Logger.Write(log); 

                        ////if (StartedCount > 0)
                        ////{
                        ////    System.Diagnostics.Debug.WriteLine("AllSubscribersStarted: Not yet");
                        ////    return false;
                        ////}
                        ////else
                        ////{
                            this.AllSubscribersDone = true;

                            ////log.Message = ("Calling function RemovefromQueue: " + MessageId.ToString());
                            ////Logger.Write(log); 

                            System.Diagnostics.Debug.WriteLine("AllSubscribersDone: yes");
                            return removeFromQueue(MessageId, this);
                        ////}
                    }
                }
                return false;
            }

        }
    }
}
