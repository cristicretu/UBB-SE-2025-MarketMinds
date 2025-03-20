using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.domain
{
    public class Image
    {
        public string url { get; set; } = string.Empty;

        public Image(string u)
        {
            url = u;
        }
    }
}
