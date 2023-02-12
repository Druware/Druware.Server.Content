using System;
using System.Collections.Generic;
using Druware.Server.Content.Entities;

namespace Druware.Server.Content
{
    public partial class Comment
    {
        public Comment()
        {
            InverseParent = new HashSet<Comment>();
        }

        public long CommentId { get; set; }
        public Guid ArticleId { get; set; }
        public Guid OwnerId { get; set; }
        public string Content { get; set; } = null!;
        public long? ParentId { get; set; }
        public DateTime Posted { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Article Article { get; set; } = null!;
        public virtual Comment? Parent { get; set; }
        public virtual ICollection<Comment> InverseParent { get; set; }
    }
}
