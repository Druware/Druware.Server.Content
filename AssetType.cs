using System;
using System.Collections.Generic;

namespace Druware.Server.Content
{
    public partial class AssetType
    {
        public AssetType()
        {
            Assets = new HashSet<Asset>();
        }

        public int TypeId { get; set; }
        public string Description { get; set; } = null!;

        public virtual ICollection<Asset> Assets { get; set; }
    }
}
