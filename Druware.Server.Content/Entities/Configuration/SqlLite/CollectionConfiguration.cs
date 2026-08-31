using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration.Sqlite;

public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> entity)
    {
        entity.ToTable("collection", "content");

        entity.Property(e => e.CollectionId)
            .HasColumnName("collection_id")
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
            .HasMaxLength(255)
            .HasColumnName("name");

        entity.Property(e => e.Description)
            .HasColumnName("description");

        entity.Property(e => e.Created)
            .HasColumnName("created")
            .HasColumnType("datetime")
            .HasDefaultValueSql("date('now')");

        entity.Property(e => e.Updated)
            .HasColumnName("updated")
            .HasColumnType("datetime")
            .HasDefaultValueSql("date('now')");

        // configure additional index settings
        entity.HasIndex(u => u.Name)
            .IsUnique();

        entity.HasKey(e => e.CollectionId);
    }
}
