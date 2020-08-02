using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic;
//using FSharpEntities;

namespace Interface
{
    public class Interface
    {

        public User GetUserbyUserName(string UserName)
        {
            BusinessLogic.UserManagerService service = new UserManagerService();
            Entities.aspnet_Users u = service.GetUserByUserName(UserName);
            User user = new User();
            user.UserName = u.UserName;
            user.Email = u.aspnet_Membership.Email;

            //var b = new Book("Name", "Author", 3, "ISBN");

            return user;

        }
    }
}
