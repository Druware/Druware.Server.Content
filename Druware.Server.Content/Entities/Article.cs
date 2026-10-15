using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Druware.Server.Content;
using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
// using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Druware.Server.Content.Entities
{
    public partial class Article
    {
        public Article()
        {
            ArticleTags = new HashSet<ArticleTag>();
            ArticleMeta = new HashSet<ArticleMeta>();
        }

        public Guid? ArticleId { get; set; } = Guid.NewGuid(); // add a default in case the Db does not support ( Sqlite ) 
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string Body { get; set; } = null!;
        public Guid? AuthorId { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Posted { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? Modified { get; set; }
        public DateTime? Expires { get; set; }
        public bool Pinned { get; set; }
        public string? Permalink { get; set; }
        public string? ByLine { get; set; }
        
        // Version 2.0 Properties
        
        public long? HeaderImageId { get; set; }
        [NotMapped]
        public Asset? HeaderImage { get; private set; }  
        
        public long? IconId { get; set; }
        [NotMapped]
        public Asset? IconImage { get; private set; }
        
        public bool IsFeatured { get; set; } = false;

        // Two takes, each optional and one line; see NormalizeTake and
        // ValidateTakesAndMeta
        public string? EddieTake { get; set; }
        public string? DruTake { get; set; }

        [JsonIgnore]
        public virtual ICollection<ArticleTag> ArticleTags { get; set; }

        [JsonIgnore]
        public virtual ICollection<ArticleMeta> ArticleMeta { get; set; }

        private string[]? _tags = null;
        [NotMapped]
        public string[]? Tags {
            get
            {
                if (_tags != null) return _tags;

                // otherwise, build the result from the ArticleTags
                List<string> list = [];
                foreach (var at in ArticleTags)
                    if (at.Tag?.Name != null) list.Add(at.Tag!.Name);
                _tags = list.ToArray();
                return _tags;
            }
            set => _tags = value;        
        }

        // whatever was last assigned to Meta, normally by a request body
        private Dictionary<string, string>? _meta = null;

        /// <summary>
        /// SEO meta tags as property/value pairs. Until something is
        /// assigned, it projects the ArticleMeta rows (Include "ArticleMeta"
        /// first), ordered by property so the JSON is stable. It is not
        /// cached, so it follows ApplyMeta.
        /// </summary>
        [NotMapped]
        public Dictionary<string, string>? Meta
        {
            get
            {
                if (_meta != null) return _meta;

                Dictionary<string, string> list = new();
                foreach (var am in ArticleMeta.OrderBy(m => m.Property, StringComparer.Ordinal))
                    if (am.Property != null) list[am.Property] = am.Value ?? "";
                return list;
            }
            set => _meta = value;
        }

        /// <summary>
        /// The meta a request body carried, or null when it omitted meta or
        /// sent null. Meta itself never reads as null, because it falls back
        /// to the rows, so this is how a controller tells "leave the meta
        /// alone" from "replace it".
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public Dictionary<string, string>? SubmittedMeta => _meta;
    }

    public partial class Article
    {
        public const int TakeMaxLength = 280;

        /// <summary>
        /// Trims a take; a blank or whitespace-only take becomes null.
        /// </summary>
        public static string? NormalizeTake(string? take) =>
            string.IsNullOrWhiteSpace(take) ? null : take.Trim();

        /// <summary>
        /// Checks a take as NormalizeTake would store it, returning a message
        /// suitable for a 400 response, or null when it is valid. The length
        /// is in UTF-16 code units (string.Length), which is what the
        /// nvarchar(280) column counts on SQL Server.
        /// </summary>
        public static string? ValidateTake(string label, string? take)
        {
            var normal = NormalizeTake(take);
            if (normal == null) return null;
            if (normal.Contains('\r') || normal.Contains('\n'))
                return $"{label} must be a single line";
            if (normal.Length > TakeMaxLength)
                return $"{label} must be at most {TakeMaxLength} characters";
            return null;
        }

        /// <summary>
        /// Checks the meta a request body carried, returning a message
        /// suitable for a 400 response, or null when it is valid or absent.
        /// Keys are trimmed and must be non-empty, at most 255 characters and
        /// unique without regard to case (SQL Server's unique index ignores
        /// case); values must be at most 2048 characters. Keys starting with
        /// '$' are skipped, because ApplyMeta drops them.
        /// </summary>
        public static string? ValidateMeta(IReadOnlyDictionary<string, string>? meta)
        {
            if (meta == null) return null;

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (rawKey, value) in meta)
            {
                var key = rawKey.Trim();
                if (key.StartsWith('$')) continue;
                if (key.Length == 0)
                    return "Meta keys must not be blank";
                if (key.Length > Entities.ArticleMeta.PropertyMaxLength)
                    return "Meta keys must be at most " +
                           $"{Entities.ArticleMeta.PropertyMaxLength} characters";
                if (!seen.Add(key))
                    return $"Meta key '{key}' appears more than once";
                if (value?.Length > Entities.ArticleMeta.ValueMaxLength)
                    return $"Meta value for '{key}' must be at most " +
                           $"{Entities.ArticleMeta.ValueMaxLength} characters";
            }
            return null;
        }

        /// <summary>
        /// Checks the takes and the meta a POST or PUT body carries,
        /// returning the first message suitable for a 400 response, or null.
        /// </summary>
        public static string? ValidateTakesAndMeta(Article model) =>
            ValidateTake("Eddie's take", model.EddieTake) ??
            ValidateTake("Dru's take", model.DruTake) ??
            ValidateMeta(model.SubmittedMeta);

        /// <summary>
        /// Makes the article's meta rows match the given set: adds new
        /// properties, updates changed values, and removes the rows whose
        /// property is missing, through the context so SaveChanges deletes
        /// them; {} therefore clears them all. Null does nothing, which is
        /// how a PUT without meta leaves the meta alone. Keys are trimmed and
        /// matched to rows without regard to case, so a change of case
        /// updates the row; a null value is stored as an empty string.
        /// Keys starting with '$' are dropped: the host serialises with
        /// ReferenceHandler.Preserve, so the meta in a response carries
        /// "$id", and an Admin app that sends a record back must not fail on
        /// it; no real meta tag name starts with '$'. Afterwards Meta
        /// projects the rows. Throws ArgumentException when ValidateMeta
        /// would refuse the set.
        /// </summary>
        public static void ApplyMeta(
            DbContext context,
            Article article,
            IReadOnlyDictionary<string, string>? meta)
        {
            if (meta == null) return;

            var invalid = ValidateMeta(meta);
            if (invalid != null) throw new ArgumentException(invalid, nameof(meta));

            // a tracked article read without Include("ArticleMeta") would
            // otherwise gain duplicate rows instead of updating its own
            var entry = context.Entry(article);
            var rowsEntry = entry.Collection(a => a.ArticleMeta);
            if (entry.State is (EntityState.Unchanged or EntityState.Modified) &&
                !rowsEntry.IsLoaded)
                rowsEntry.Load();

            var rows = article.ArticleMeta.ToList();
            foreach (var (rawKey, value) in meta)
            {
                var key = rawKey.Trim();
                if (key.StartsWith('$')) continue;

                var row = rows.FirstOrDefault(r => string.Equals(
                    r.Property, key, StringComparison.OrdinalIgnoreCase));
                if (row == null)
                {
                    article.ArticleMeta.Add(new ArticleMeta
                    {
                        Article = article,
                        Property = key,
                        Value = value ?? ""
                    });
                    continue;
                }

                row.Property = key;
                row.Value = value ?? "";
                rows.Remove(row);
            }

            // whatever is left was not in the new set
            foreach (var row in rows)
            {
                context.Remove(row);
                article.ArticleMeta.Remove(row);
            }

            article._meta = null;
        }

        public static Article? ByPermalinkOrId(
            ContentContext context,
            string permalink)
        {
            Guid id;
            Article? article = null;

            if (Guid.TryParse(permalink, out id))
                article = context.News?
                    .Include("ArticleTags.Tag")
                    .Include("ArticleMeta")
                    .Include("HeaderImage")
                    .Include("IconImage")
                    .SingleOrDefault(t => t.ArticleId == id);

            article ??= context.News?
                .Include("ArticleTags.Tag")
                .Include("ArticleMeta")
                .Include("HeaderImage")
                .Include("IconImage")
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
