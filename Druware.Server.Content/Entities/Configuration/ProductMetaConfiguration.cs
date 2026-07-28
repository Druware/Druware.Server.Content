using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class ProductMetaConfiguration : IEntityTypeConfiguration<ProductMeta>
{
    public void Configure(EntityTypeBuilder<ProductMeta> entity)
    {
        entity.HasKey(e => e.Id)
            .HasName("product_meta_pkey");

        entity.ToTable("product_meta", "content");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.ProductId)
            .HasColumnName("product_id");

        entity.Property(e => e.Property)
            .HasColumnName("property")
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.Value)
            .HasColumnName("value")
            .HasMaxLength(2048);

        entity.HasOne(d => d.Product)
            .WithMany(p => p.ProductMeta)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientCascade)
            .HasConstraintName("fk_productmeta_productid__product_productid");

        entity.HasIndex(e => new { e.ProductId, e.Property })
            .IsUnique()
            .HasDatabaseName("ix_product_meta_product_id_property");
    }
}
