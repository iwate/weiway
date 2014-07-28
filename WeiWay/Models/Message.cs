using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiWay.Models
{
    public class Message : BaseModel
    {
        public string Text { get; set; }
        //public string Descrition { get; set; }
        public int Index { get; set; }
        public bool IsPending { get; set; }
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}