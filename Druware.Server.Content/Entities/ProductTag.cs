using Druware.Server.Entities;

namespace Druware.Server.Content.Entities;

public class ProductTag
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public long TagId { get; set; }

    public virtual Product? Product { get; set; } = null;
    public virtual Tag? Tag { get; set; } = null;
}