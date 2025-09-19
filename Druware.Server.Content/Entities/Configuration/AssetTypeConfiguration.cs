using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration
{
    public class AssetTypeConfiguration : IEntityTypeConfiguration<AssetType>
    {
        public void Configure(EntityTypeBuilder<AssetType> entity)
        {
            entity.HasKey(e => e.TypeId)
                .HasName("asset_type_pkey");

            entity.ToTable("asset_type", "content");

            entity.Property(e => e.TypeId).HasColumnName("type_id");

            entity.Property(e => e.Description)
                .HasMaxLength(128)
                .HasColumnName("description");            
        }
    }
}