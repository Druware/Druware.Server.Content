using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration.Sqlite
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> entity)
        {
            entity.ToTable("article", "content");

            entity.Property(e => e.ArticleId)
                .HasColumnName("article_id"); 
                // .HasDefaultValueSql("newid()"); // Sqlite does not support UID's version

            entity.Property(e => e.AuthorId)
                .HasColumnName("author_id");

            entity.Property(e => e.Body)
                .HasColumnName("body");

            entity.Property(e => e.ByLine)
                .HasColumnName("by_line")
                .HasMaxLength(255);

            entity.Property(e => e.Expires)
                .HasColumnName("expires")
                .HasColumnType("datetime");

            entity.Property(e => e.Modified)
                .HasColumnName("modified")
                .HasColumnType("datetime")
                .HasDefaultValueSql("date('now')");

            entity.Property(e => e.Permalink)
                .HasColumnName("permalink")
                .HasMaxLength(255);

            entity.Property(e => e.Pinned)
                .HasColumnName("pinned");

            entity.Property(e => e.Posted)
                .HasColumnName("posted")
                .HasColumnType("datetime")
                .HasDefaultValueSql("date('now')");

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
                .WithMany()
                .HasForeignKey(d => d.HeaderImageId);
            entity.HasOne(d => d.Icon)
                .WithMany()
                .HasForeignKey(d => d.IconId);
        }
    }
}