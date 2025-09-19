using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration.Microsoft;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.ToTable("product", "content");

        entity.Property(e => e.ProductId)
            .HasColumnName("product_id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
            .HasMaxLength(255)
            .HasColumnName("name");

        entity.Property(e => e.Short)
            .HasMaxLength(32)
            .HasColumnName("short");

        entity.Property(e => e.Summary)
            .HasColumnName("summary")
            .HasMaxLength(2048);

        entity.Property(e => e.Description)
            .HasColumnName("description")
            .HasColumnType("varchar(max)");

        entity.Property(e => e.Created)
            .HasColumnName("created")
            .HasColumnType("datetime")
            .HasDefaultValueSql("getDate()");

        entity.Property(e => e.Updated)
            .HasColumnName("updated")
            .HasColumnType("datetime")
            .HasDefaultValueSql("getDate()");

        entity.Property(e => e.License)
            .HasColumnName("license")
            .HasColumnType("varchar(max)");


        entity.Property(e => e.DownloadUrl)
            .HasColumnName("download_url")
            .HasMaxLength(278);

        entity.Property(e => e.DocumentationUrl)
            .HasColumnName("documentation_url")
            .HasMaxLength(278);

        entity.Property(e => e.IconUrl)
            .HasColumnName("icon_url")
            .HasMaxLength(278);

        // configure additional index settings
        entity.HasIndex(u => u.Short)
            .IsUnique();

        entity.HasKey(e => e.ProductId);
    }
}