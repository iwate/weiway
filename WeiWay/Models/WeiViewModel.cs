using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiWay.Models
{
    public class WeiViewModel
    {
        public string id { get; set; }
        public string text { get; set; }
        public string imageUrl { get; set; }
        public string userName { get; set; }
        public string userIcon { get; set; }
        public int onechances { get; set; }
        public DateTimeOffset postDate { get; set; }
        public ICollection<Comment> comments { get; set; }
    }
}