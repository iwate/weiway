using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiWay.Models
{
    public class Wei : BaseModel
    {
        public string MessageId { get; set; }
        public virtual Message Message { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}