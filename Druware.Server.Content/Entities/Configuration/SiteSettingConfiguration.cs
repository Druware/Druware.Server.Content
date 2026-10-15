using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration;

public class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
{
    public void Configure(EntityTypeBuilder<SiteSetting> entity)
    {
        entity.HasKey(e => e.SettingId)
            .HasName("site_setting_pkey");

        entity.ToTable("site_setting", "content");

        entity.Property(e => e.SettingId)
            .HasColumnName("setting_id")
            .ValueGeneratedOnAdd();

        // "key" is a reserved word in some providers
        entity.Property(e => e.Key)
            .HasColumnName("setting_key")
            .HasMaxLength(SiteSetting.KeyMaxLength)
            .IsRequired();

        entity.Property(e => e.Value)
            .HasColumnName("value");

        entity.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(SiteSetting.DescriptionMaxLength);

        entity.Property(e => e.Updated)
            .HasColumnName("updated")
            .IsRequired();

        entity.HasIndex(e => e.Key)
            .IsUnique()
            .HasDatabaseName("ix_site_setting_setting_key");
    }
}
