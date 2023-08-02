using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration
{
    public class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTag>
    {
        public void Configure(EntityTypeBuilder<ArticleTag> entity)
        {
            entity.HasKey(e => e.Id)
                .HasName("article_tag_pkey");

            entity.ToTable("article_tag", "content");

            entity.Property(e => e.Id);

            entity.Property(e => e.ArticleId);

            entity.Property(e => e.TagId);

            entity.HasOne(d => d.Article)
                .WithMany(p => p.ArticleTags)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("fk_articletags_articleid__article_articleid");

        }
    }
}

