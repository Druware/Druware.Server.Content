using System;
using System.Collections.Generic;

namespace Druware.Server.Content
{
    public partial class ArticleTag
    {
        public long TagsId { get; set; }
        public long ArticleId { get; set; }
        public long TagId { get; set; }

        public virtual Article Article { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
