using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Domain
{
    public class Image
    {
        public string url { get; set; } = string.Empty;

        public Image(string url)
        {
            this.url = url;
        }
    }
}
