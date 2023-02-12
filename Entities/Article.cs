using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Druware.Server.Content;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Druware.Server.Content.Entities
{
    public partial class Article
    {
        public Article()
        {
            ArticleTags = new HashSet<ArticleTag>();
            Comments = new HashSet<Comment>();
        }

        public Guid? ArticleId { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string Body { get; set; } = null!;
        public Guid? AuthorId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Posted { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Modified { get; set; }
        public DateTime? Expires { get; set; }
        public bool Pinned { get; set; }
        public string? Permalink { get; set; }
        public string? ByLine { get; set; }

        [JsonIgnore]
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        private ICollection<string>? _tags = null;
        [NotMapped]
        public ICollection<string>? Tags {
            get
            {
                if (_tags != null) return _tags;

                // otherwise, build the result from the ArticleTags
                List<string> list = new();
                foreach (ArticleTag at in ArticleTags)
                    list.Add(at.Tag!.Name!);
                return list;
            }
            set => _tags = value;        
        }
    }

    public partial class Article
    {
        public static Article? ByPermalinkOrId(
            ContentContext context,
            string permalink)
        {
            Guid id;
            Article? article = null;

            if (Guid.TryParse(permalink, out id))
                article = context.News?
                    .Include("ArticleTags.Tag")
                    .SingleOrDefault(t => t.ArticleId == id);

            if (article == null)
                article = context.News?
                    .Include("ArticleTags.Tag")
                    .SingleOrDefault(t => t.Permalink == permalink);

            return article;
        }

        public static bool IsPermalinkValid(
            ContentContext context,
            string permalink,
            Guid? articleId = null)
        {
            Article? article = null;
            article = articleId == null ?
                context.News?.SingleOrDefault(t => t.Permalink == permalink) :
                context.News?.SingleOrDefault(
                    t => t.Permalink == permalink && t.ArticleId != articleId);

            return (article == null);
        }
    }
}
