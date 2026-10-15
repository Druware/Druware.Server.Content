using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class ArticleMetaConfiguration : IEntityTypeConfiguration<ArticleMeta>
{
    public void Configure(EntityTypeBuilder<ArticleMeta> entity)
    {
        entity.HasKey(e => e.Id)
            .HasName("article_meta_pkey");

        entity.ToTable("article_meta", "content");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.ArticleId)
            .HasColumnName("article_id");

        entity.Property(e => e.Property)
            .HasColumnName("property")
            .HasMaxLength(ArticleMeta.PropertyMaxLength)
            .IsRequired();

        entity.Property(e => e.Value)
            .HasColumnName("value")
            .HasMaxLength(ArticleMeta.ValueMaxLength);

        // both sides named, so no provider maps it by convention; the rows
        // belong to the article, so the database removes them with it
        entity.HasOne(d => d.Article)
            .WithMany(p => p.ArticleMeta)
            .HasForeignKey(d => d.ArticleId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_articlemeta_articleid__article_articleid");

        entity.HasIndex(e => new { e.ArticleId, e.Property })
            .IsUnique()
            .HasDatabaseName("ix_article_meta_article_id_property");
    }
}
