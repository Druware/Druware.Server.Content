using System;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration
{
	public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> entity)
        {
            entity.ToTable("document", "content");

            entity.Property(e => e.DocumentId);

            entity.Property(e => e.AuthorId);

            entity.Property(e => e.Body);

            entity.Property(e => e.Modified)
                .HasColumnType("timestamp without time zone");

            entity.Property(e => e.Permalink)
                .HasColumnType("character varying");

            entity.Property(e => e.Posted)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Title)
                .HasMaxLength(255);

            // configure additional index settings
            entity.HasIndex(u => u.Permalink)
                .IsUnique();
        }
    }
}

