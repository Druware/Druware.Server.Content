using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Druware.Server.Content.Entities.Configuration.PostgreSql;

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
            .HasColumnName("description")
            .HasColumnType("text");

        entity.Property(e => e.Created)
            .HasColumnName("created")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("now()");

        entity.Property(e => e.Updated)
            .HasColumnName("updated")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("now()");

        // configure additional index settings
        entity.HasIndex(u => u.Name)
            .IsUnique();

        entity.HasKey(e => e.CollectionId);
    }
}
