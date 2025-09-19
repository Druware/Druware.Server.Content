using System;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration.Sqlite
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> entity)
        {
            entity.ToTable("document", "content");

            entity.Property(e => e.DocumentId)
                .HasColumnName("document_id");

            entity.Property(e => e.AuthorId)
                .HasColumnName("author_id");

            entity.Property(e => e.Body)
                .HasColumnName("body");

            entity.Property(e => e.Modified)
                .HasColumnName("modified")
                .HasColumnType("datetime");

            entity.Property(e => e.Permalink)
                .HasColumnName("permalink")
                .HasColumnType("varchar(278)");

            entity.Property(e => e.Posted)
                .HasColumnName("posted")
                .HasColumnType("datetime")
                .HasDefaultValueSql("date('now')");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(255);

            // configure additional index settings
            entity.HasIndex(u => u.Permalink)
                .IsUnique();
        }
    }
}