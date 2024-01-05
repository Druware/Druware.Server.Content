using System;
using System.Collections.Generic;
using Druware.Server.Content.Entities;
using Newtonsoft.Json;

// TODO: The AssetType Foreign Key is not yet implemented.

namespace Druware.Server.Content
{
    public partial class Asset
    {
        public long AssetId { get; set; }
        public int TypeId { get; set; }
        public string Description { get; set; } = null!;
        [JsonIgnore]
        public byte[]? Content { get; set; }
        public string? MediaType { get; set; }
        public string? FileName { get; set; }

        public virtual AssetType Type { get; set; } = null!;
    }

    public partial class Asset
    {
        public static Asset? ByFileNameOrId(
            ContentContext context,
            string? lookup)
        {
            Asset? asset = null;
            if (int.TryParse(lookup, out int id))
                asset = context.Assets?
//                    .Include("ArticleTags.Tag")
                    .SingleOrDefault(t => t.AssetId == id);

            return asset ??= context.Assets?
                // .Include("ArticleTags.Tag")
                .SingleOrDefault(t => t.FileName == lookup);
        }
        
        public static bool IsFileNameAvailable(
            ContentContext context,
            string lookup) =>
            ByFileNameOrId(context, lookup) == null;
        
    }
}
