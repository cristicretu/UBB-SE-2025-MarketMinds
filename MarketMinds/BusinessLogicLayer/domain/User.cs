using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Domain
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string userType { get; set; } = string.Empty;
        public float balance { get; set; }
        public float rating { get; set; }
        public float password { get; set; }


    }
}
