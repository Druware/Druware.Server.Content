using System;
using System.Collections.Generic;

namespace Druware.Server.Content
{
    public partial class Article
    {
        public Article()
        {
            ArticleTags = new HashSet<ArticleTag>();
            Comments = new HashSet<Comment>();
            // Tags = new List<string>();
        }

        public long ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string Body { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public DateTime Posted { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Expires { get; set; }
        public bool Pinned { get; set; }
        public string? Permalink { get; set; }
        public string? ByLine { get; set; }

        public virtual ICollection<ArticleTag> ArticleTags { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        // public ICollection<string> Tags { get; set; }
    }
}
