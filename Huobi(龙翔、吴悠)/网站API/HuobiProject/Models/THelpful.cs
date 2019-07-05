using System;
using System.Collections.Generic;

namespace HuobiProject.Models
{
    public partial class THelpful
    {
        public string HelpfulId { get; set; }
        public string ArticleLangId { get; set; }
        public short Helpful { get; set; }
        public string Ip { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual TArticleLang ArticleLang { get; set; }
    }
}
