using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class ReviewTagConfiguration : IEntityTypeConfiguration<ReviewTag>
{
    public void Configure(EntityTypeBuilder<ReviewTag> entity)
    {
        entity.HasKey(e => e.Id)
            .HasName("review_tag_pkey");

        entity.ToTable("review_tag", "content");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.ReviewId)
            .HasColumnName("review_id");

        entity.Property(e => e.TagId)
            .HasColumnName("tag_id");

        // both sides named, so no provider maps either by convention; the
        // rows belong to the review, so the database removes them with it
        entity.HasOne(d => d.Review)
            .WithMany(p => p.ReviewTags)
            .HasForeignKey(d => d.ReviewId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_reviewtags_reviewid__review_reviewid");

        entity.HasOne(d => d.Tag)
            .WithMany()
            .HasForeignKey(d => d.TagId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_reviewtags_tagid__tag_tagid");
    }
}
