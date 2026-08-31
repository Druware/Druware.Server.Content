namespace Druware.Server.Content.Entities;

public class CollectionProduct
{
    public long Id { get; set; }
    public long CollectionId { get; set; }
    public long ProductId { get; set; }

    public virtual Collection? Collection { get; set; } = null;
    public virtual Product? Product { get; set; } = null;
}
