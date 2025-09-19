using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> entity)
    {
        entity.ToTable("asset", "content");

        entity.Property(e => e.AssetId).HasColumnName("asset_id");

        entity.Property(e => e.Content).HasColumnName("content");

        entity.Property(e => e.Description)
            .HasMaxLength(255)
            .HasColumnName("description");

        entity.Property(e => e.FileName)
            .HasMaxLength(192)
            .HasColumnName("file_name");

        entity.Property(e => e.MediaType)
            .HasMaxLength(128)
            .HasColumnName("media_type");

        entity.Property(e => e.TypeId).HasColumnName("type_id");

        entity.HasOne(d => d.Type)
            .WithMany(p => p.Assets)
            .HasForeignKey(d => d.TypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName(
                "fk_content_asset_type_id__content_asset_type_type_id");
    }
}
