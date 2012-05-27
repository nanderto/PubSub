using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phantom.PubSub
{
    /// <summary>
    /// This class just creates a list of I subscribers of the generic type.
    /// It is a collection class that currently has no specialised behavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public class Subscribers<T> : List<ISubscriber<T>>
    //{
    //    // public List<ISubscriber<T>> GetCopyofSubscribers()
    //    ////{
    //    ////    int TrackIfStartedLength = 0;
    //    ////    List<ISubscriber<T>> newSubsribers = new List<ISubscriber<T>>();

    //    ////    lock (this)
    //    ////    {
    //    ////        ISubscriber<T>[] newSubscriber = new ISubscriber<T>[this.Count];
    //    ////        this.CopyTo(newSubscriber);
    //    ////        TrackIfStartedLength = newSubscriber.Length;

    //    ////        for (int i = 0; i < TrackIfStartedLength; i++)
    //    ////        {
    //    ////            newSubsribers.Add(newSubscriber[i]);
    //    ////        }
    //    ////    }
    //    ////    return newSubsribers;
    //    ////}
    //}
}
