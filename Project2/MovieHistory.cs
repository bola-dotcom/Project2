using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
   public class MovieHistory
    {
        public string title { get; set; }
        public int year { get; set; }
        public List<string> genre { get; set; }
        public string Genre => string.Join(",", genre);
        public string emoji { get; set; }

        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
