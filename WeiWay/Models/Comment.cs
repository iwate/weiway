using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiWay.Models
{
    public class Comment : BaseModel
    {
        public string MessageId { get; set; }
        public virtual Message Message { get; set; }
        public string WeiId { get; set; }
        [JsonIgnore]
        public virtual Wei Wei { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}