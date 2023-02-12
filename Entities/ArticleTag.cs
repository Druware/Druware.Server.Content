using System;
using System.Collections.Generic;
using Druware.Server.Entities;

namespace Druware.Server.Content.Entities
{
    public partial class ArticleTag
    {
        public long Id { get; set; }
        public Guid? ArticleId { get; set; }
        public long TagId { get; set; }

        public virtual Article? Article { get; set; } = null;
        public virtual Druware.Server.Entities.Tag? Tag { get; set; } = null;
    }
}
