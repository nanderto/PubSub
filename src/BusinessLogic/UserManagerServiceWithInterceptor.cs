using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interceptor;

namespace BusinessLogic
{
    public class UserManagerServiceWithInterceptor : BusinessLogic.IUserManagerServiceWithInterceptor
    {
        [Service]
        public void SaveUser(string name)
        {
            var user = new Entities.aspnet_Users();
            user.UserName = name;
        }
    }
}
