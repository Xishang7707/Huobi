using System;
using System.Collections.Generic;

namespace HuobiProject.Models
{
    public partial class TCommentLike
    {
        public string CommentLikeId { get; set; }
        public string UserId { get; set; }
        public string CommentId { get; set; }
        public short Like { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual TComment Comment { get; set; }
        public virtual TUser User { get; set; }
    }
}
