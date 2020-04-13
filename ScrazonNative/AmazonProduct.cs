using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrazonNative
{
    class AmazonProduct
    {        
        public string Title { get; set; }
        public string URL { get; set; }
        public float Rating { get; set; }
        public int TotalReviews { get; set; }
        public int RecentReviews { get; set; }
        public float Percentage { get; set; }
    }
}
