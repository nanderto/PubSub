using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Data
{
    public class Repository
    {
        public List<global::Entities.aspnet_Applications> GetApplications()
        {
            Entities.aspnetdbEntities context = new Entities.aspnetdbEntities();
            return context.aspnet_Applications.ToList<Entities.aspnet_Applications>();


        }

        public Entities.aspnet_Users GetUserByUserName(string UserName)
        {
            Entities.aspnetdbEntities context = new Entities.aspnetdbEntities();
            return context.aspnet_Users.Where(x => x.UserName == UserName).Single();
        }
    }
}
