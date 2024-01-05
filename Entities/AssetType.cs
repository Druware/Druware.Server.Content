namespace Druware.Server.Content.Entities
{
    public partial class AssetType
    {
        /*public AssetType()
        {
        }*/

        public int? TypeId { get; set; } = null;
        public string? Description { get; set; } = null;
        public ICollection<Asset>? Assets { get; set; } = null;
    }

    public partial class AssetType
    {
        public static AssetType? ById(
            ContentContext context,
            int lookup)
        {
            return context.AssetTypes?
                .SingleOrDefault(t => t.TypeId == lookup);
        }
    }
}
