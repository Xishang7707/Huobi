using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuobiProject.Models
{
    public partial class TTicket
    {
        public TTicket()
        {
            TTicketChat = new HashSet<TTicketChat>();
        }

        public string TicketId { get; set; }
        public int TypeId { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public int StatusId { get; set; }
        public DateTime ActiveTime { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual TType Status { get; set; }
        public virtual TType Type { get; set; }
        public virtual TUser User { get; set; }

        [JsonIgnore]
        public virtual ICollection<TTicketChat> TTicketChat { get; set; }
    }
}
