using System;
using System.Collections.Generic;
using Druware.Server.Entities;

namespace Druware.Server.Content.Entities
{
    public partial class DocumentTag
    {
        public long Id { get; set; }
        public Guid? DocumentId { get; set; }
        public long TagId { get; set; }

        public virtual Document? Document { get; set; } = null;
        public virtual Druware.Server.Entities.Tag? Tag { get; set; } = null;
    }
}
