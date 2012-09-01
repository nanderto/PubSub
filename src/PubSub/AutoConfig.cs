using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Phantom.PubSub
{
    public static class AutoConfig<T>
    {
        public static readonly ReadOnlyCollection<Tuple<string, Type, TimeSpan>> SubscriberInfos = null;

        static AutoConfig()
        {
            var subscriberInfoTypes = AutoConfig<T>.GetSubscriberInfos<T>(AssemblyLocator<T>.SubscribersInBin);
            SubscriberInfos = new ReadOnlyCollection<Tuple<string, Type, TimeSpan>>(subscriberInfoTypes);
        }

        public static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            System.Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        private static List<Tuple<string, Type, TimeSpan>> GetSubscriberInfos<T>(ReadOnlyCollection<Type> types)
        {
            List< Tuple<string, Type, TimeSpan>> subscriberInfos = new List<Tuple<string, Type, TimeSpan>>();
            
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
                        //seriously hacky using this to get an object created with out a Parameterless constructor
                        //because I dont want developers to have to add parameterless constructors to their Subscriber<T>
                        //implementations
                        //what happens of the subscriber they create is not Serializable? dunno
                        var something = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(item);
                        timeSpan = ((Subscriber<T>)something).DefaultTimeToExpire;

                        //I might be able to get this done but not sure
                        //var obj = Activator.CreateInstance(item, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.OptionalParamBinding, null, new Object[] { Type.Missing }, null); 
                        //timeSpan = (TimeSpan)ts.GetValue(Activator.CreateInstance(item));

                        //var obj = Activator.CreateInstance(item, new Object[]{});
                        //timeSpan = ((Subscriber<T>)obj).DefaultTimeToExpire;
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
