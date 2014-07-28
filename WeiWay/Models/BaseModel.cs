using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiWay.Models
{
    public abstract class BaseModel
    {
        public string Id { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset UpdateTime { get; set; }
        public BaseModel()
        {
            Id = Guid.NewGuid().ToString();
            CreateTime = DateTimeOffset.Now;
            UpdateTime = DateTimeOffset.Now;
        }
    }
}