using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration.PostgreSql
{
	public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> entity)
        {
            entity.ToTable("article", "content");

            entity.Property(e => e.ArticleId)
                .HasColumnName("article_id")
                .HasDefaultValueSql("gen_random_uuid()"); // Microsoft version

            entity.Property(e => e.AuthorId)
                .HasColumnName("author_id");

            entity.Property(e => e.Body)
                .HasColumnName("body");

            entity.Property(e => e.ByLine)
                .HasColumnName("by_line")
                .HasMaxLength(255);

            entity.Property(e => e.Expires)
                .HasColumnName("expires")
                .HasColumnType("timestamp without time zone");

            entity.Property(e => e.Modified)
                .HasColumnName("modified")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Permalink)
                .HasColumnName("permalink")
                .HasMaxLength(255);

            entity.Property(e => e.Pinned)
                .HasColumnName("pinned");

            entity.Property(e => e.Posted)
                .HasColumnName("posted")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.Summary)
                .HasColumnName("summary")
                .HasMaxLength(2048);

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(255);
            
            // Version 2.0 Added Properties
            
            entity.Property(e => e.HeaderImageId)
                .HasColumnName("header_image_id");
            entity.Property(e => e.IconId)
                .HasColumnName("icon_id");
            entity.Property(e => e.IsFeatured)
                .HasColumnName("is_featured");

            // configure additional index settings
            entity.HasIndex(u => u.Permalink)
                .IsUnique();
            
            // Version 2.0 Added Relationships
            
            entity.HasOne(d => d.HeaderImage)
                .WithOne()
                .HasForeignKey<Article>(d => d.HeaderImageId);
            entity.HasOne(d => d.IconImage)
                .WithOne()
                .HasForeignKey<Article>(d => d.IconId);
        
        }
    }
}


