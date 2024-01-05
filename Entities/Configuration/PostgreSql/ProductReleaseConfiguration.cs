
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration.PostgreSql;

public class ProductReleaseConfiguration : IEntityTypeConfiguration<ProductRelease>
{
    public void Configure(EntityTypeBuilder<ProductRelease> entity)
    {
        entity.ToTable("product_release", "content");

        entity.Property(e => e.ReleaseId)
            .HasColumnName("release_id")
            .ValueGeneratedOnAdd();
        
        entity.Property(e => e.ProductId)
            .HasColumnName("product_id");

        entity.Property(e => e.Title)
            .HasMaxLength(255)
            .HasColumnName("title");

        entity.Property(e => e.Body)
            .HasColumnType("text)")
            .HasColumnName("body");

        entity.Property(e => e.AuthorId)
            .HasColumnName("author_id");

        entity.Property(e => e.Posted)
            .HasColumnName("posted")
//            .HasColumnType("datetime")
            .HasDefaultValueSql("now()");

        entity.Property(e => e.Modified)
            .HasColumnName("modified")
//            .HasColumnType("datetime")
            .HasDefaultValueSql("now()");

        entity.Property(e => e.DownloadUrl)
            .HasColumnName("download_url")
            .HasMaxLength(278);
        
        entity.HasKey(e => e.ReleaseId);
        
        entity.HasOne(d => d.Product)
            .WithMany(p => p.History)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(
                "fk_content_product_release_product_id__content_product_product_id");
    }
}