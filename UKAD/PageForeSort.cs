using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKAD
{
    class PageForeSort
    {
        public string link { get; set; }
        public long time { get; set; }
    }
    class PageComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return (new CaseInsensitiveComparer()).Compare(((PageForeSort)x).time, ((PageForeSort)y).time);
        }
    }
}
