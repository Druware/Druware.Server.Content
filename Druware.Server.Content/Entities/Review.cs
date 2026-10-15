using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Druware.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Druware.Server.Content.Entities;

/// <summary>
/// A gear review: one product, an optional photo, and up to two scores and
/// two takes, named for the two Two Wheel Junction riders. New reviews are
/// drafts until IsPublished is set.
/// </summary>
public partial class Review
{
    public const int TitleMaxLength = 255;
    public const int PermalinkMaxLength = 255;
    public const int ProductNameMaxLength = 200;
    public const int CategoryMaxLength = 64;
    public const int SummaryMaxLength = 1024;
    public const int ProductUrlMaxLength = 2048;
    public const int ScoreMin = 1;
    public const int ScoreMax = 10;

    public Review()
    {
        ReviewTags = new HashSet<ReviewTag>();
    }

    public long? ReviewId { get; set; } = null;
    public string Title { get; set; } = string.Empty;
    public string? Permalink { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Summary { get; set; }
    public string? Body { get; set; }
    public long? PhotoAssetId { get; set; }
    public string? ProductUrl { get; set; }
    public bool IsAffiliate { get; set; } = false;

    // whole numbers from ScoreMin to ScoreMax; null is not scored yet
    public int? EddieScore { get; set; }
    public int? DruScore { get; set; }

    // one line each, with Article's rules; see Article.NormalizeTake
    public string? EddieTake { get; set; }
    public string? DruTake { get; set; }

    [JsonIgnore]
    public virtual ICollection<ReviewTag> ReviewTags { get; set; }

    private string[]? _tags = null;
    [NotMapped]
    public string[]? Tags {
        get
        {
            if (_tags != null) return _tags;

            // otherwise, build the result from the ReviewTags
            List<string> list = [];
            foreach (var rt in ReviewTags)
                if (rt.Tag?.Name != null) list.Add(rt.Tag!.Name);
            _tags = list.ToArray();
            return _tags;
        }
        set => _tags = value;
    }

    // both are set by the server, never by the database
    public DateTime? Posted { get; set; }
    public DateTime? Modified { get; set; }

    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
}

public partial class Review
{
    /// <summary>
    /// Finds a review by id when the value is a whole number, otherwise by
    /// permalink, including its tags. An unpublished review is only found
    /// when includeUnpublished is true.
    /// </summary>
    public static Review? ByPermalinkOrId(
        IContentContext context,
        string value,
        bool includeUnpublished = true)
    {
        var query = context.Reviews?
            .Include("ReviewTags.Tag")
            .Where(r => includeUnpublished || r.IsPublished);
        if (query == null) return null;

        if (long.TryParse(value, out var id))
            return query.SingleOrDefault(r => r.ReviewId == id);

        return query.SingleOrDefault(r => r.Permalink == value);
    }

    /// <summary>
    /// The reviews for a list, with their tags, newest posted first and then
    /// by descending id. Drafts are left out unless includeUnpublished is
    /// true.
    /// </summary>
    public static IQueryable<Review> List(
        IContentContext context,
        bool includeUnpublished)
    {
        if (context.Reviews == null)
            return Enumerable.Empty<Review>().AsQueryable();

        return context.Reviews
            .Include("ReviewTags.Tag")
            .Where(r => includeUnpublished || r.IsPublished)
            .OrderByDescending(r => r.Posted)
            .ThenByDescending(r => r.ReviewId);
    }

    /// <summary>
    /// True when no other review uses the permalink. Pass the review's own id
    /// when it is being updated, so it does not collide with itself.
    /// </summary>
    public static bool IsPermalinkValid(
        IContentContext context,
        string permalink,
        long? reviewId = null) =>
        context.Reviews?.Any(r =>
            r.Permalink == permalink &&
            (reviewId == null || r.ReviewId != reviewId)) != true;

    /// <summary>
    /// Builds a permalink from a title with the rule NewsController.Add uses
    /// for articles.
    /// </summary>
    public static string GeneratePermalink(string title) =>
        title.ToLower()
            .Replace(" ", "_")
            .Replace("!", "")
            .Replace(",", "")
            .Replace("\"", "")
            .Replace("?", "")
            .Replace("=", "")
            .EncodeUrl();

    /// <summary>
    /// Tidies what a request body carried before it is checked and stored:
    /// the takes as Article.NormalizeTake leaves them, and a blank permalink
    /// or product URL as null.
    /// </summary>
    public static void Normalize(Review review)
    {
        review.EddieTake = Article.NormalizeTake(review.EddieTake);
        review.DruTake = Article.NormalizeTake(review.DruTake);
        if (string.IsNullOrWhiteSpace(review.Permalink)) review.Permalink = null;
        if (string.IsNullOrWhiteSpace(review.ProductUrl)) review.ProductUrl = null;
    }

    /// <summary>
    /// Fills a missing or blank permalink from the title with
    /// GeneratePermalink, so a review always has a page. Run it after
    /// Normalize and before Validate, which checks the result; with a blank
    /// title it does nothing and Validate refuses the title instead.
    /// </summary>
    public static void EnsurePermalink(Review review)
    {
        if (string.IsNullOrWhiteSpace(review.Permalink) &&
            !string.IsNullOrWhiteSpace(review.Title))
            review.Permalink = GeneratePermalink(review.Title);
    }

    /// <summary>
    /// Checks the fields a caller supplies, returning the first message
    /// suitable for a 400 response, or null when the review is valid. It does
    /// not check that the permalink is unused; see IsPermalinkValid.
    /// </summary>
    public static string? Validate(Review review)
    {
        if (string.IsNullOrWhiteSpace(review.Title))
            return "Title is required";
        if (review.Title.Length > TitleMaxLength)
            return $"Title must be at most {TitleMaxLength} characters";

        if (string.IsNullOrWhiteSpace(review.ProductName))
            return "Product name is required";
        if (review.ProductName.Length > ProductNameMaxLength)
            return $"Product name must be at most {ProductNameMaxLength} characters";

        if (review.Permalink != null)
        {
            // GET reads a number as an id, so a numeric permalink is unreachable
            if (review.Permalink.All(char.IsAsciiDigit))
                return "Permalink cannot be only digits";
            if (review.Permalink.Length > PermalinkMaxLength)
                return $"Permalink must be at most {PermalinkMaxLength} characters";
        }

        if (review.Category?.Length > CategoryMaxLength)
            return $"Category must be at most {CategoryMaxLength} characters";
        if (review.Summary?.Length > SummaryMaxLength)
            return $"Summary must be at most {SummaryMaxLength} characters";

        if (review.ProductUrl != null)
        {
            if (review.ProductUrl.Length > ProductUrlMaxLength)
                return $"Product URL must be at most {ProductUrlMaxLength} characters";
            if (!Uri.TryCreate(review.ProductUrl, UriKind.Absolute, out var url) ||
                (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps))
                return "Product URL must be a full http or https address";
        }

        return ValidateScore("Eddie's score", review.EddieScore) ??
               ValidateScore("Dru's score", review.DruScore) ??
               Article.ValidateTake("Eddie's take", review.EddieTake) ??
               Article.ValidateTake("Dru's take", review.DruTake);
    }

    private static string? ValidateScore(string label, int? score) =>
        score is null or (>= ScoreMin and <= ScoreMax)
            ? null
            : $"{label} must be a whole number from {ScoreMin} to {ScoreMax}";
}
