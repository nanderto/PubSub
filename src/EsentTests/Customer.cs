using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsentTests
{
    [Serializable]
    public class Customer
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// test class because tests create a file for each business object you should only use these objects in one test to ensure that 
    /// the tests do not interfere with each other
    /// </summary>
    [Serializable]
    public class Customer1
    {
        public string Name { get; set; }
    }

    [Serializable]
    public class CustomerX
    {
        public string Name { get; set; }
    }

    [Serializable]
    public class CustomerY
    {
        public string Name { get; set; }
    }
}
