using System;
using System.Collections.Generic;

namespace HuobiProject.Models
{
    public partial class TAttention
    {
        public string AttentionId { get; set; }
        public string UserId { get; set; }
        public int TypeId { get; set; }
        public DateTime? CreateTime { get; set; }

        public virtual TType Type { get; set; }
        public virtual TUser User { get; set; }
    }
}
