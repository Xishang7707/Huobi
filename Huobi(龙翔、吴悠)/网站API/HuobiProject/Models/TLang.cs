using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuobiProject.Models
{
    public partial class TLang
    {
        public TLang()
        {
            TType = new HashSet<TType>();
        }

        public int LangId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<TType> TType { get; set; }
    }
}
