using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeiWay.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<SelectListItem> WeiWei { get; set; }
        public string UserName { get; set; }
        public string UserIcon { get; set; }
    }
}