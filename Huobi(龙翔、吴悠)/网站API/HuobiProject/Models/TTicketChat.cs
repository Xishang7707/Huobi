using System;
using System.Collections.Generic;

namespace HuobiProject.Models
{
    public partial class TTicketChat
    {
        public string TicketChatId { get; set; }
        public string TicketId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string Annex { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual TResource AnnexNavigation { get; set; }
        public virtual TTicket Ticket { get; set; }
        public virtual TUser User { get; set; }
    }
}
