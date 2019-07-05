using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuobiProject.Models
{
    public partial class TArticle
    {
        public TArticle()
        {
            TArticleLang = new HashSet<TArticleLang>();
        }

        public string ArticleId { get; set; }
        public DateTime CreateTime { get; set; }

        [JsonIgnore]
        public virtual ICollection<TArticleLang> TArticleLang { get; set; }
    }
}
