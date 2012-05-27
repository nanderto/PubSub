namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class is strictly for test perposes. Shoul implement perfomce counters instead
    /// </summary>
    public static class Counter
    {
        private static object TotalSubscriberLockObject = new object();
        
        private static object listofCountersLockObject = new object();
        
        private static int totalSubscriberCount = 0;
        
        private static int[] listofCounters = new int[6];
        
        public static int Subscriber(int counternumber)
        {
            lock (TotalSubscriberLockObject)
            {
                return listofCounters[counternumber];
            }
        }

        public static int TotalSubscriberCount()
        {
            lock (TotalSubscriberLockObject)
            {
                return totalSubscriberCount;
            }
        }

        public static void Increment(int index)
        {
            lock (TotalSubscriberLockObject)
            {
                listofCounters[index]++;
                totalSubscriberCount++;
            }
        }
    }
}
