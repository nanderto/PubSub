using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snap;
using Castle.DynamicProxy;
using System.Reflection;

namespace Interceptor
{
    public class ServiceInterceptor :MethodInterceptor
    {
        public override void InterceptMethod(IInvocation invocation, MethodBase method, Attribute attribute)
        {
            System.Diagnostics.Debug.Write("made it here");

            invocation.Proceed(); // the underlying method call
        }
       
    }
}
