using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Druware.Server.Content.Entities
{
    public partial class Document
    {
        public Guid? DocumentId { get; set; }
        public string? Title { get; set; } = null!;
        public string? Body { get; set; } = null!;
        public Guid? AuthorId { get; set; }
        public DateTime? Posted { get; set; }
        public DateTime? Modified { get; set; }
        public string? Permalink { get; set; }

        public Document()
        {
            DocumentTags = new HashSet<DocumentTag>();
        }

        [JsonIgnore]
        public virtual ICollection<DocumentTag>? DocumentTags { get; set; }

        private ICollection<string>? _tags = null;
        [NotMapped]
        public ICollection<string>? Tags
        {
            get
            {
                if (_tags != null) return _tags;

                // otherwise, build the result from the ArticleTags
                List<string> list = new();
                foreach (DocumentTag at in DocumentTags!)
                    list.Add(at.Tag!.Name!);
                return list;
            }
            set => _tags = value;
        }
    }

    public partial class Document
    {
        public static Document? ByPermalinkOrId(
            ContentContext context,
            string permalink)
        {
            Guid id;
            Document? documents = null;

            if (Guid.TryParse(permalink, out id))
                documents = context.Documents?
                    .Include("DocumentTags.Tag")
                    .SingleOrDefault(t => t.DocumentId == id);

            if (documents == null)
                documents = context.Documents?
                    .Include("DocumentTags.Tag")
                    .SingleOrDefault(t => t.Permalink == permalink);

            return documents;
        }

        public static bool IsPermalinkValid(
            ContentContext context,
            string permalink,
            Guid? documentId = null)
        {
            Document? documents = null;
            documents = documentId == null ?
                context.Documents?.SingleOrDefault(t => t.Permalink == permalink) :
                context.Documents?.SingleOrDefault(
                    t => t.Permalink == permalink && t.DocumentId != documentId);

            return (documents == null);
        }
    }
}
