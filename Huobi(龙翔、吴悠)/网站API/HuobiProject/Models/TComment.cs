using System;
using System.Collections.Generic;

namespace HuobiProject.Models
{
    public partial class TComment
    {
        public TComment()
        {
            TCommentLike = new HashSet<TCommentLike>();
        }

        public string CommentId { get; set; }
        public string UserId { get; set; }
        public string ArticleLangId { get; set; }
        public string Content { get; set; }
        public DateTime ActiveTime { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual TArticleLang ArticleLang { get; set; }
        public virtual TUser User { get; set; }
        public virtual ICollection<TCommentLike> TCommentLike { get; set; }
    }
}
