using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;

namespace BusinessLogic
{
    public class UserManagerService
    {


        public Entities.aspnet_Users GetUserByUserName(string UserName)
        {
            Data.Repository data = new Data.Repository();
            return data.GetUserByUserName(UserName);
        }
    }


}
