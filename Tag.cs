using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Druware.Server.Content
{
    public partial class Tag
    {
        public Tag()
        {
            //    ArticleTags = new HashSet<ArticleTag>();
        }

        [JsonIgnore]
        public long TagId { get; set; }
        public string Name { get; set; } = null!;

        internal static Tag? ByNameOrId(EntityContext context, string value)
        {
            Int32 id;
            Tag? tag = null;

            if (Int32.TryParse(value, out id))
                tag = context.Tags?.Single(t => t.TagId == id);

            if (tag == null)
                tag = context.Tags?.Single(t => t.Name == value);

            return tag;
        }

        // public virtual ICollection<ArticleTag> ArticleTags { get; set; }


    }

}
