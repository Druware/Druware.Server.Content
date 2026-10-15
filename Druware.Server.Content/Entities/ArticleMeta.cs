namespace Druware.Server.Content.Entities;

/// <summary>
/// One SEO meta tag on an Article, as a property/value pair, mirroring
/// ProductMeta. The public shape is Article.Meta.
/// </summary>
public class ArticleMeta
{
    public const int PropertyMaxLength = 255;
    public const int ValueMaxLength = 2048;

    public long Id { get; set; }
    public Guid ArticleId { get; set; }
    public string? Property { get; set; } = null;
    public string? Value { get; set; } = null;

    public virtual Article? Article { get; set; } = null;
}
