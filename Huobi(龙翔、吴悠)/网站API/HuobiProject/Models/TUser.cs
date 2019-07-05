using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuobiProject.Models
{
    public partial class TUser
    {
        public TUser()
        {
            TArticleLang = new HashSet<TArticleLang>();
            TAttention = new HashSet<TAttention>();
            TComment = new HashSet<TComment>();
            TCommentLike = new HashSet<TCommentLike>();
            TTicket = new HashSet<TTicket>();
            TTicketChat = new HashSet<TTicketChat>();
        }

        public string UserId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public short Super { get; set; }
        public string UserName { get; set; }
        public string Photo { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual TResource PhotoNavigation { get; set; }

        [JsonIgnore]
        public virtual ICollection<TArticleLang> TArticleLang { get; set; }
        [JsonIgnore]
        public virtual ICollection<TAttention> TAttention { get; set; }
        [JsonIgnore]
        public virtual ICollection<TComment> TComment { get; set; }
        [JsonIgnore]
        public virtual ICollection<TCommentLike> TCommentLike { get; set; }
        [JsonIgnore]
        public virtual ICollection<TTicket> TTicket { get; set; }
        [JsonIgnore]
        public virtual ICollection<TTicketChat> TTicketChat { get; set; }
    }
}
