using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HuobiProject.Models
{
    public partial class TArticleLang
    {
        public TArticleLang()
        {
            TComment = new HashSet<TComment>();
            THelpful = new HashSet<THelpful>();
        }

        public string ArticleLangId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public short Commentable { get; set; }
        public short Flag { get; set; }
        public string ArticleId { get; set; }
        public string UserId { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime CreateTime { get; set; }
        public int TypeId { get; set; }

        public virtual TArticle Article { get; set; }

        public virtual TType Type { get; set; }

        public virtual TUser User { get; set; }

        [JsonIgnore]
        public virtual ICollection<TComment> TComment { get; set; }

        [JsonIgnore]
        public virtual ICollection<THelpful> THelpful { get; set; }
    }
}
