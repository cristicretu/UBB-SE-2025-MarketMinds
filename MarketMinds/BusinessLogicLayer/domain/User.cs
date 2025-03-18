using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.domain
{
    internal class User
    {
        internal int _id { get; set; }
        internal string _username { get; set; } = string.Empty;
        internal string _email { get; set; } = string.Empty;
        internal string _userType { get; set; } = string.Empty;
        internal float _balance { get; set; }
        internal float _rating { get; set; }
        internal float _password { get; set; }


    }
}
