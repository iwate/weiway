using Newtonsoft.Json;
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
        public string ImageUrl { get; set; }
        [JsonIgnore]
        public virtual ICollection<ApplicationUser> OneChanceUsers { get; set; }
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }
        public Wei()
        {
            OneChanceUsers = new HashSet<ApplicationUser>();
            Comments = new HashSet<Comment>();
        }
    }
}