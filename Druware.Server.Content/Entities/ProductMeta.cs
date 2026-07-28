namespace Druware.Server.Content.Entities;

public class ProductMeta
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string? Property { get; set; } = null;
    public string? Value { get; set; } = null;

    public virtual Product? Product { get; set; } = null;
}
