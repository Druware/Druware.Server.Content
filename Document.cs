using System;
using System.Collections.Generic;

namespace Druware.Server.Content
{
    public partial class Document
    {
        public long DocumentId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public long AuthorId { get; set; }
        public DateTime Posted { get; set; }
        public DateTime? Modified { get; set; }
        public string? Permalink { get; set; }
    }
}
