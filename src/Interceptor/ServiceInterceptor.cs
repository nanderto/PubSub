using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snap;
using Castle.DynamicProxy;
using System.Reflection;
using Phantom.PubSub;

namespace Interceptor
{
    public class ServiceInterceptor<T> : MethodInterceptor
    {
        //private IPublishSubscribeChannel<T> PublishSubscribeChannel;

        //public ServiceInterceptor(IPublishSubscribeChannel<T> PublishSubscribeChannel)
        //{
        //    this.PublishSubscribeChannel = PublishSubscribeChannel;
        //}

        public override void InterceptMethod(IInvocation invocation, MethodBase method, Attribute attribute)
        {
            System.Diagnostics.Debug.Write("made it here");

            invocation.Proceed(); // the underlying method call
        }
       
    }
}
