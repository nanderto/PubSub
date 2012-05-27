﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Snap;
using Snap.CastleWindsor;
using BusinessLogic;

namespace Interceptor
{
    public static class SnapConfigurator
    {
        public readonly static WindsorContainer _container;

         static SnapConfigurator()
        {
            _container = new WindsorContainer();
        }

        public static void Configurator()
        {
           

            SnapConfiguration.For(new CastleAspectContainer(_container.Kernel)).Configure(c =>
            {
                c.IncludeNamespace("Interceptor");
                c.Bind<ServiceInterceptor>().To<ServiceAttribute>();
            });

            _container.AddComponent("UserManagerServiceWithInterceptor", typeof(IUserManagerServiceWithInterceptor), typeof(UserManagerServiceWithInterceptor));
        }
    }

}
