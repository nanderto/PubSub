//-----------------------------------------------------------------------
// <copyright file="Counter.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Enum CounterType
    /// </summary>
    public enum CounterType
    {
        /// <summary>
        /// Count for subscribers
        /// </summary>
        Subscribers = 0,
        
        /// <summary>
        /// Count for The number removed from queues
        /// </summary>
        RemovedFromQueues = 3,
        
        /// <summary>
        /// Count for calling Processing A batch
        /// </summary>
        ProcessingABatch = 4,
        
        /// <summary>
        /// Count for calling is the queue empty
        /// </summary>
        IsEmptyCheck = 5,
    }

    /// <summary>
    /// This class is strictly for test purposes. Should implement perfomce counters instead
    /// </summary>
    public static class Counter
    {
        /// <summary>
        /// Lock object for the Total number of subscribers
        /// </summary>
        private static object totalSubscriberLockObject = new object();
                
        /// <summary>
        /// Total number of subscribers associated with this specific type
        /// </summary>
        private static int totalSubscriberCount = 0;
        
        /// <summary>
        /// Collection of counters to monitor the usage of the component
        /// </summary>
        private static int[] listofCounters = new int[30];
        
        /// <summary>
        /// Locks the counter object and gets the current Total Subscriber Count
        /// </summary>
        /// <param name="counterNumber">Position in list of the specified counter to return</param>
        /// <returns>List of counters for debugging purposes</returns>
        public static int Subscriber(int counterNumber)
        {
            lock (totalSubscriberLockObject)
            {
                return listofCounters[counterNumber];
            }
        }

        /// <summary>
        /// Total number of subscribers
        /// </summary>
        /// <returns>Total subscriber count</returns>
        public static int TotalSubscriberCount()
        {
            lock (totalSubscriberLockObject)
            {
                return totalSubscriberCount;
            }
        }

        /// <summary>
        /// Increment the specified counter in the collection of counters
        /// </summary>
        /// <param name="index">Position in list of the specified counter to update</param>
        public static void Increment(int index)
        {
            lock (totalSubscriberLockObject)
            {
                listofCounters[index]++;
                totalSubscriberCount++;
            }
        }

        /// <summary>
        /// Increments the specified counter type.
        /// </summary>
        /// <param name="counterType">Type of the counter.</param>
        public static void Increment(CounterType counterType)
        {
            Increment((int)counterType);
        }
    }
}
