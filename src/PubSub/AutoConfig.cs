//-----------------------------------------------------------------------
// <copyright file="AutoConfig.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class AutoConfig gets a list of data for each subscriber that it finds in the bin, the data includes the Name
    /// the type, so the type can be instantiated, and the time span until the subscriber expires.
    /// Subscribers must inherit from <see cref="ISubscriber {T}"/> the T matches the specialization of this class
    /// </summary>
    /// <typeparam name="T">Autoconfig is specialized to this type T</typeparam>
    internal static class AutoConfig<T>
    {
        /// <summary>
        /// Read only collection of Subscriber data
        /// </summary>
        public static readonly ReadOnlyCollection<Tuple<string, Type, TimeSpan>> SubscriberInfos = null;
        
        /// <summary>
        /// Initializes static members of the <see cref="AutoConfig{T}" /> class.
        /// </summary>
        static AutoConfig()
        {
            var subscriberInfoTypes = AutoConfig<T>.GetSubscriberInfos<T>(AssemblyLocator<T>.SubscribersInBin);
            SubscriberInfos = new ReadOnlyCollection<Tuple<string, Type, TimeSpan>>(subscriberInfoTypes);
        }

        /// <summary>
        /// Gets the subscriber data including name Type and time until subscriber will expire.
        /// </summary>
        /// <typeparam name="T">Specialization for this class</typeparam>
        /// <param name="types">List of types found.</param>
        /// <returns>List of subscriber data retrieved from input list</returns>
        internal static List<Tuple<string, Type, TimeSpan>> GetSubscriberInfos<T>(ReadOnlyCollection<Type> types)
        {
            List<Tuple<string, Type, TimeSpan>> subscriberInfos = new List<Tuple<string, Type, TimeSpan>>();

            foreach (var item in types)
            {
                var ts = item.GetProperty("DefaultTimeToExpire");

                if (ts != null)
                {
                    ISubscriber<T> subscriber = null;
                    TimeSpan timeSpan = default(TimeSpan);
                    if (item.ContainsGenericParameters)
                    {
                        var typeArg = typeof(T);
                        var t = item.MakeGenericType(typeArg);
                        subscriber = (ISubscriber<T>)Activator.CreateInstance(t);
                        timeSpan = subscriber.DefaultTimeToExpire;
                    }
                    else
                    {
                        ////seriously hacky using this to get an object created with out a Parameterless constructor
                        ////because I dont want developers to have to add parameterless constructors to their Subscriber<T>
                        ////implementations
                        ////what happens of the subscriber they create is not Serializable? dunno
                        var something = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(item);
                        timeSpan = ((Subscriber<T>)something).DefaultTimeToExpire;

                        ////I might be able to get this done but not sure
                        ////var obj = Activator.CreateInstance(item, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding, null, new Object[] { Type.Missing }, null); 
                        ////timeSpan = (TimeSpan)ts.GetValue(Activator.CreateInstance(item));

                        ////var obj = Activator.CreateInstance(item, new Object[]{});
                        ////timeSpan = ((Subscriber<T>)obj).DefaultTimeToExpire;
                    }
                    
                    subscriberInfos.Add(new Tuple<string, Type, TimeSpan>(item.Name, item, timeSpan));
                }
                else
                {
                    subscriberInfos.Add(new Tuple<string, Type, TimeSpan>(item.Name, item, new TimeSpan(0, 0, 100)));
                }
            }
            
            return subscriberInfos;
        }
    }
}
