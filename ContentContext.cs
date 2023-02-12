using System;
using System.Collections.Generic;
using System.Linq;
using Druware.Server.Content.Entities;
using Druware.Server.Content.Entities.Configuration;
using Druware.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// NOTE: If you rebuild the migration, you may need to go into the .cs and
//       remove the Tag clause, as it may try to create the table even though
//       it already exists

namespace Druware.Server.Content
{
    public partial class ContentContext : DbContext
    {
#if DEBUG
        private const string cs = "Host=localhost;Database=druware;Username=postgres;Password=notforproduction";
#endif

        public ContentContext()
        {
        }

        public ContentContext(DbContextOptions<ContentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> News { get; set; } = null!;
        public virtual DbSet<ArticleTag> ArticleTags { get; set; } = null!;

        public virtual DbSet<AssetType> AssetTypes { get; set; } = null!;
        public virtual DbSet<Asset> Assets { get; set; } = null!;
        
        public virtual DbSet<Comment> Comments { get; set; } = null!;

        public virtual DbSet<Document> Documents { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            // We *should* never get here, and only in DEBUG mode
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(cs); // this is the default
#else
            throw new Exception("No Connection String Provided");
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ArticleConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleTagConfiguration());


            // TODO: Migrate the rest to Configuration design for code clarity


            modelBuilder.Entity<Asset>(entity =>
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
                    .HasConstraintName("fk_content_asset_type_id__content_asset_type_type_id");
            });

            modelBuilder.Entity<AssetType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("asset_type_pkey");

                entity.ToTable("asset_type", "content");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(128)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment", "content");

                entity.Property(e => e.CommentId).HasColumnName("comment_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.Modified)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Posted)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("posted")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_news_comment_article_id__news_article_article_id");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("fk_news_comment_parent_id__news_comment_comment_id");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("document", "content");

                entity.Property(e => e.DocumentId).HasColumnName("document_id");

                entity.Property(e => e.AuthorId).HasColumnName("author_id");

                entity.Property(e => e.Body).HasColumnName("body");

                entity.Property(e => e.Modified)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified");

                entity.Property(e => e.Permalink)
                    .HasColumnType("character varying")
                    .HasColumnName("permalink");

                entity.Property(e => e.Posted)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("posted")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public static void ConfigureSecurityRoles(Druware.Server.ServerContext context)
        {
            if (context != null)
            {
                if (context.Roles.FirstOrDefault<IdentityRole<string>>(r => r.Name == NewsSecurityRole.Author) == null)
                    context.Roles.Add(
                        new Role
                        {
                            Description = "News Author",
                            Name = NewsSecurityRole.Author,
                            NormalizedName = NewsSecurityRole.Author.ToUpper()
                        });
                if (context.Roles.FirstOrDefault<IdentityRole<string>>(r => r.Name == NewsSecurityRole.Editor) == null)
                    context.Roles.Add(
                        new Role
                        {
                            Description = "News Editor",
                            Name = NewsSecurityRole.Editor,
                            NormalizedName = NewsSecurityRole.Editor.ToUpper()
                        });


                context.SaveChanges();
            }
        }
    }
}
