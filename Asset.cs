using System;
using System.Collections.Generic;

namespace Druware.Server.Content
{
    public partial class Asset
    {
        public long AssetId { get; set; }
        public int TypeId { get; set; }
        public string Description { get; set; } = null!;
        public byte[]? Content { get; set; }
        public string? MediaType { get; set; }
        public string? FileName { get; set; }

        public virtual AssetType Type { get; set; } = null!;
    }
}
