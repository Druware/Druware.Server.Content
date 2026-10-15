using Druware.Server.Entities;

namespace Druware.Server.Content.Entities;

/// <summary>
/// Joins a Review to a Tag from the shared tag pool, the way ArticleTag
/// does for articles. The public shape is Review.Tags.
/// </summary>
public class ReviewTag
{
    public long Id { get; set; }
    public long ReviewId { get; set; }
    public long TagId { get; set; }

    public virtual Review? Review { get; set; } = null;
    public virtual Tag? Tag { get; set; } = null;
}
