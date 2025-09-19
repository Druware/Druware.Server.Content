using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// TODO: Move back up a namespace, as there is nothing platform specific

namespace Druware.Server.Content.Entities.Configuration
{
    public class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTag>
    {
        public void Configure(EntityTypeBuilder<ArticleTag> entity)
        {
            entity.HasKey(e => e.Id)
                .HasName("article_tag_pkey");

            entity.ToTable("article_tag", "content");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.ArticleId)
                .HasColumnName("article_id");

            entity.Property(e => e.TagId)
                .HasColumnName("tag_id");

            entity.HasOne(d => d.Article)
                .WithMany(p => p.ArticleTags)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("fk_articletags_articleid__article_articleid");

        }
    }
}

