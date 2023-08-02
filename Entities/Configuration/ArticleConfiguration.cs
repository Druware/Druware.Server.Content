using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration
{
	public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> entity)
        {
            entity.ToTable("article", "content");

            entity.Property(e => e.ArticleId)
                .HasDefaultValueSql("gen_random_uuid ()"); // PostgreSQL version

            entity.Property(e => e.AuthorId);

            entity.Property(e => e.Body);

            entity.Property(e => e.ByLine)
                .HasMaxLength(255);

            entity.Property(e => e.Expires)
                .HasColumnType("timestamp without time zone");

            entity.Property(e => e.Modified)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Permalink)
                .HasMaxLength(255);

            entity.Property(e => e.Pinned);

            entity.Property(e => e.Posted)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Summary)
                .HasMaxLength(2048);

            entity.Property(e => e.Title)
                .HasMaxLength(255);

            // configure additional index settings
            entity.HasIndex(u => u.Permalink)
                .IsUnique();
        }
    }
}


