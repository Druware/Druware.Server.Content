using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> entity)
    {
        entity.HasKey(e => e.ReviewId)
            .HasName("review_pkey");

        entity.ToTable("review", "content");

        entity.Property(e => e.ReviewId)
            .HasColumnName("review_id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Title)
            .HasColumnName("title")
            .HasMaxLength(Review.TitleMaxLength)
            .IsRequired();

        entity.Property(e => e.Permalink)
            .HasColumnName("permalink")
            .HasMaxLength(Review.PermalinkMaxLength);

        entity.Property(e => e.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(Review.ProductNameMaxLength)
            .IsRequired();

        entity.Property(e => e.Category)
            .HasColumnName("category")
            .HasMaxLength(Review.CategoryMaxLength);

        entity.Property(e => e.Summary)
            .HasColumnName("summary")
            .HasMaxLength(Review.SummaryMaxLength);

        entity.Property(e => e.Body)
            .HasColumnName("body");

        entity.Property(e => e.PhotoAssetId)
            .HasColumnName("photo_asset_id");

        entity.Property(e => e.ProductUrl)
            .HasColumnName("product_url")
            .HasMaxLength(Review.ProductUrlMaxLength);

        entity.Property(e => e.IsAffiliate)
            .HasColumnName("is_affiliate");

        entity.Property(e => e.EddieScore)
            .HasColumnName("eddie_score");
        entity.Property(e => e.DruScore)
            .HasColumnName("dru_score");

        entity.Property(e => e.EddieTake)
            .HasColumnName("eddie_take")
            .HasMaxLength(Article.TakeMaxLength);
        entity.Property(e => e.DruTake)
            .HasColumnName("dru_take")
            .HasMaxLength(Article.TakeMaxLength);

        // the server stamps both, so neither has a database default
        entity.Property(e => e.Posted)
            .HasColumnName("posted");
        entity.Property(e => e.Modified)
            .HasColumnName("modified");

        entity.Property(e => e.IsPublished)
            .HasColumnName("is_published");
        entity.Property(e => e.IsFeatured)
            .HasColumnName("is_featured");

        entity.HasIndex(e => e.Permalink)
            .IsUnique()
            .HasDatabaseName("ix_review_permalink");

        // no navigation; removing the asset leaves the review without a photo
        entity.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(e => e.PhotoAssetId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_review_photoassetid__asset_assetid");
    }
}
