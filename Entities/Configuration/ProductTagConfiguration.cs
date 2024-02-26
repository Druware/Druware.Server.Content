using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> entity)
    {
        entity.HasKey(e => e.Id)
            .HasName("product_tag_pkey");

        entity.ToTable("product_tag", "content");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.ProductId)
            .HasColumnName("product_id");

        entity.Property(e => e.TagId)
            .HasColumnName("tag_id");

        entity.HasOne(d => d.Product)
            .WithMany(p => p.ProductTags)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientCascade)
            .HasConstraintName("fk_producttags_productid__product_productid");
    }
}