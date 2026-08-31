using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class CollectionProductConfiguration : IEntityTypeConfiguration<CollectionProduct>
{
    public void Configure(EntityTypeBuilder<CollectionProduct> entity)
    {
        entity.HasKey(e => e.Id)
            .HasName("collection_product_pkey");

        entity.ToTable("collection_product", "content");

        entity.Property(e => e.Id)
            .HasColumnName("id");

        entity.Property(e => e.CollectionId)
            .HasColumnName("collection_id");

        entity.Property(e => e.ProductId)
            .HasColumnName("product_id");

        entity.HasOne(d => d.Collection)
            .WithMany(p => p.CollectionProducts)
            .HasForeignKey(d => d.CollectionId)
            .OnDelete(DeleteBehavior.ClientCascade)
            .HasConstraintName("fk_collectionproducts_collectionid__collection_collectionid");

        entity.HasOne(d => d.Product)
            .WithMany(p => p.CollectionProducts)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.ClientCascade)
            .HasConstraintName("fk_collectionproducts_productid__product_productid");

        entity.HasIndex(e => new { e.CollectionId, e.ProductId })
            .IsUnique()
            .HasDatabaseName("ix_collection_product_collection_id_product_id");
    }
}
