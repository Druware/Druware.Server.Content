using System.Text.Json.Serialization;

namespace Druware.Server.Content.Entities;

public partial class ProductRelease
{
	public long? ReleaseId { get; set; } = null;
	public long? ProductId { get; set; } = null;
	public string? Title { get; set; } = null;
	public string? Body { get; set; } = null;
	public Guid? AuthorId { get; set; } = null;
	public DateTime? Posted { get; set; } = null;
	public DateTime? Modified { get; set; } = null;
	public string? DownloadUrl { get; set; } = null;
	
	[JsonIgnore]
	public Product? Product { get; set; }
}

public partial class ProductRelease
{
	public static ProductRelease? ById(
		ContentContext context,
		string? lookup)
	{
		ProductRelease? r = null;
		if (int.TryParse(lookup, out var id))
			r = context.ProductReleases?
				.SingleOrDefault(t => t.ReleaseId == id);
		return r;
	}
}