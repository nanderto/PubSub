using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snap;

namespace Interceptor
{
    public class InterceptedService
    {
       

        [Service]
        public void SaveUser(string UserName)
        {
            string user = UserName;
        }
    }
}
