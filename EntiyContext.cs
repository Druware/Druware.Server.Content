using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Druware.Server.Content
{
    public partial class EntityContext : DbContext
    {
        public EntityContext()
        {
        }

        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> News { get; set; } = null!;
        public virtual DbSet<ArticleTag> ArticleTags { get; set; } = null!;
        public virtual DbSet<Asset> Assets { get; set; } = null!;
        public virtual DbSet<AssetType> AssetTypes { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Database=druware;Username=postgres;Password=gr8orthan0");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("article", "content");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.AuthorId).HasColumnName("author_id");

                entity.Property(e => e.Body).HasColumnName("body");

                entity.Property(e => e.ByLine)
                    .HasMaxLength(255)
                    .HasColumnName("by_line");

                entity.Property(e => e.Expires)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("expires");

                entity.Property(e => e.Modified)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("modified");

                entity.Property(e => e.Permalink)
                    .HasMaxLength(255)
                    .HasColumnName("permalink");

                entity.Property(e => e.Pinned).HasColumnName("pinned");

                entity.Property(e => e.Posted)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("posted")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Summary)
                    .HasMaxLength(2014)
                    .HasColumnName("summary");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<ArticleTag>(entity =>
            {
                entity.HasKey(e => e.TagsId)
                    .HasName("article_tag_pkey");

                entity.ToTable("article_tag", "content");

                entity.Property(e => e.TagsId).HasColumnName("tags_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");

                entity.Property(e => e.TagId).HasColumnName("tag_id");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleTags)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_news_article_tag__news_atricle_article_id");

                //entity.HasOne(d => d.Tag)
                //    .WithMany(p => p.ArticleTags)
                //    .HasForeignKey(d => d.TagId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("fk_news_article_tag__article_tags_tag_id");
            });

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

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag", "content");

                entity.Property(e => e.TagId).HasColumnName("tag_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
