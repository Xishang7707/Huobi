using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuobiProject.Models
{
    public partial class TType
    {
        public TType()
        {
            InverseParent = new HashSet<TType>();
            TArticleLang = new HashSet<TArticleLang>();
            TAttention = new HashSet<TAttention>();
            TTicketStatus = new HashSet<TTicket>();
            TTicketType = new HashSet<TTicket>();
        }

        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public int? LangId { get; set; }

        public virtual TLang Lang { get; set; }
        public virtual TType Parent { get; set; }

        [JsonIgnore]
        public virtual ICollection<TType> InverseParent { get; set; }
        [JsonIgnore]
        public virtual ICollection<TArticleLang> TArticleLang { get; set; }
        [JsonIgnore]
        public virtual ICollection<TAttention> TAttention { get; set; }
        [JsonIgnore]
        public virtual ICollection<TTicket> TTicketStatus { get; set; }
        [JsonIgnore]
        public virtual ICollection<TTicket> TTicketType { get; set; }
    }
}
