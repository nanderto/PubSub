using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
     [Serializable]
    public class User
    {
       
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; } 
    }
}
