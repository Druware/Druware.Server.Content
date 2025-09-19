using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration
{
    public class DocumentTagConfiguration : IEntityTypeConfiguration<DocumentTag>
    {
        public void Configure(EntityTypeBuilder<DocumentTag> entity)
        {
            entity.HasKey(e => e.Id)
                .HasName("document_tag_pkey");

            entity.ToTable("document_tag", "content");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.DocumentId)
                .HasColumnName("document_id");

            entity.Property(e => e.TagId)
                .HasColumnName("tag_id");

            entity.HasOne(d => d.Document)
                .WithMany(p => p.DocumentTags)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("fk_documenttags_documentid__document_documentid");

        }
    }
}

