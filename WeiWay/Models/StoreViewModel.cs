using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiWay.Models
{
    public class StoreViewModel
    {
        public string UserIcon { get; set; }
        public string UserName { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}