using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuobiProject.Models
{
    public partial class TResource
    {
        public TResource()
        {
            TTicketChat = new HashSet<TTicketChat>();
            TUser = new HashSet<TUser>();
        }

        public string ResourceId { get; set; }
        public string Value { get; set; }
        public int TypeId { get; set; }
        public string ContentType { get; set; }
        public DateTime? CreateTime { get; set; }

        [JsonIgnore]
        public virtual ICollection<TTicketChat> TTicketChat { get; set; }
        [JsonIgnore]
        public virtual ICollection<TUser> TUser { get; set; }
    }
}
