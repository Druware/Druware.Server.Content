﻿// <auto-generated />
using System;
using Druware.Server.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Druware.Server.Content.Migrations.Microsoft
{
    [DbContext(typeof(ContentContextMicrosoft))]
    partial class ContentContextMicrosoftModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Druware.Server.Content.Asset", b =>
                {
                    b.Property<long>("AssetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("asset_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AssetId"), 1L, 1);

                    b.Property<byte[]>("Content")
                        .HasColumnType("varbinary(max)")
                        .HasColumnName("content");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("description");

                    b.Property<string>("FileName")
                        .HasMaxLength(192)
                        .HasColumnType("nvarchar(192)")
                        .HasColumnName("file_name");

                    b.Property<string>("MediaType")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("media_type");

                    b.Property<int>("TypeId")
                        .HasColumnType("int")
                        .HasColumnName("type_id");

                    b.HasKey("AssetId");

                    b.HasIndex("TypeId");

                    b.ToTable("asset", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Article", b =>
                {
                    b.Property<Guid?>("ArticleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("article_id")
                        .HasDefaultValueSql("newid()");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("author_id");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("body");

                    b.Property<string>("ByLine")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("by_line");

                    b.Property<DateTime?>("Expires")
                        .HasColumnType("datetime")
                        .HasColumnName("expires");

                    b.Property<DateTime?>("Modified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime")
                        .HasColumnName("modified")
                        .HasDefaultValueSql("getDate()");

                    b.Property<string>("Permalink")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("permalink");

                    b.Property<bool>("Pinned")
                        .HasColumnType("bit")
                        .HasColumnName("pinned");

                    b.Property<DateTime?>("Posted")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime")
                        .HasColumnName("posted")
                        .HasDefaultValueSql("getDate()");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)")
                        .HasColumnName("summary");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("title");

                    b.HasKey("ArticleId");

                    b.HasIndex("Permalink")
                        .IsUnique()
                        .HasFilter("[permalink] IS NOT NULL");

                    b.ToTable("article", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ArticleTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<Guid?>("ArticleId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("article_id");

                    b.Property<long>("TagId")
                        .HasColumnType("bigint")
                        .HasColumnName("tag_id");

                    b.HasKey("Id")
                        .HasName("article_tag_pkey");

                    b.HasIndex("ArticleId");

                    b.HasIndex("TagId");

                    b.ToTable("article_tag", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.AssetType", b =>
                {
                    b.Property<int?>("TypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("type_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("TypeId"), 1L, 1);

                    b.Property<string>("Description")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("description");

                    b.HasKey("TypeId")
                        .HasName("asset_type_pkey");

                    b.ToTable("asset_type", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Document", b =>
                {
                    b.Property<Guid?>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("document_id");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("author_id");

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("body");

                    b.Property<DateTime?>("Modified")
                        .HasColumnType("datetime")
                        .HasColumnName("modified");

                    b.Property<string>("Permalink")
                        .HasColumnType("varchar(278)")
                        .HasColumnName("permalink");

                    b.Property<DateTime?>("Posted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("posted")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("title");

                    b.HasKey("DocumentId");

                    b.HasIndex("Permalink")
                        .IsUnique()
                        .HasFilter("[permalink] IS NOT NULL");

                    b.ToTable("document", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.DocumentTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<Guid?>("DocumentId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("document_id");

                    b.Property<long>("TagId")
                        .HasColumnType("bigint")
                        .HasColumnName("tag_id");

                    b.HasKey("Id")
                        .HasName("document_tag_pkey");

                    b.HasIndex("DocumentId");

                    b.HasIndex("TagId");

                    b.ToTable("document_tag", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Product", b =>
                {
                    b.Property<long?>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("ProductId"), 1L, 1);

                    b.Property<DateTime?>("Created")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime")
                        .HasColumnName("created")
                        .HasDefaultValueSql("getDate()");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("DocumentationUrl")
                        .HasMaxLength(278)
                        .HasColumnType("nvarchar(278)")
                        .HasColumnName("documentation_url");

                    b.Property<string>("DownloadUrl")
                        .HasMaxLength(278)
                        .HasColumnType("nvarchar(278)")
                        .HasColumnName("download_url");

                    b.Property<string>("IconUrl")
                        .HasMaxLength(278)
                        .HasColumnType("nvarchar(278)")
                        .HasColumnName("icon_url");

                    b.Property<string>("License")
                        .HasColumnType("varchar(max)")
                        .HasColumnName("license");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("name");

                    b.Property<string>("Short")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("short");

                    b.Property<string>("Summary")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)")
                        .HasColumnName("summary");

                    b.Property<DateTime?>("Updated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime")
                        .HasColumnName("updated")
                        .HasDefaultValueSql("getDate()");

                    b.HasKey("ProductId");

                    b.HasIndex("Short")
                        .IsUnique()
                        .HasFilter("[short] IS NOT NULL");

                    b.ToTable("product", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ProductRelease", b =>
                {
                    b.Property<long?>("ReleaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("release_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("ReleaseId"), 1L, 1);

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("author_id");

                    b.Property<string>("Body")
                        .HasColumnType("varchar(max)")
                        .HasColumnName("body");

                    b.Property<string>("DownloadUrl")
                        .HasMaxLength(278)
                        .HasColumnType("nvarchar(278)")
                        .HasColumnName("download_url");

                    b.Property<DateTime?>("Modified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("modified")
                        .HasDefaultValueSql("getDate()");

                    b.Property<DateTime?>("Posted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("posted")
                        .HasDefaultValueSql("getDate()");

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("title");

                    b.HasKey("ReleaseId");

                    b.HasIndex("ProductId");

                    b.ToTable("product_release", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ProductTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    b.Property<long>("TagId")
                        .HasColumnType("bigint")
                        .HasColumnName("tag_id");

                    b.HasKey("Id")
                        .HasName("product_tag_pkey");

                    b.HasIndex("ProductId");

                    b.HasIndex("TagId");

                    b.ToTable("product_tag", "content");
                });

            modelBuilder.Entity("Druware.Server.Entities.Tag", b =>
                {
                    b.Property<long?>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("tag_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("TagId"), 1L, 1);

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("name");

                    b.HasKey("TagId");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[name] IS NOT NULL");

                    b.ToTable("tag", (string)null);
                });

            modelBuilder.Entity("Druware.Server.Content.Asset", b =>
                {
                    b.HasOne("Druware.Server.Content.Entities.AssetType", "Type")
                        .WithMany("Assets")
                        .HasForeignKey("TypeId")
                        .IsRequired()
                        .HasConstraintName("fk_content_asset_type_id__content_asset_type_type_id");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ArticleTag", b =>
                {
                    b.HasOne("Druware.Server.Content.Entities.Article", "Article")
                        .WithMany("ArticleTags")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .HasConstraintName("fk_articletags_articleid__article_articleid");

                    b.HasOne("Druware.Server.Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.DocumentTag", b =>
                {
                    b.HasOne("Druware.Server.Content.Entities.Document", "Document")
                        .WithMany("DocumentTags")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .HasConstraintName("fk_documenttags_documentid__document_documentid");

                    b.HasOne("Druware.Server.Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Document");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ProductRelease", b =>
                {
                    b.HasOne("Druware.Server.Content.Entities.Product", "Product")
                        .WithMany("History")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("fk_content_product_release_product_id__content_product_product_id");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ProductTag", b =>
                {
                    b.HasOne("Druware.Server.Content.Entities.Product", "Product")
                        .WithMany("ProductTags")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("fk_producttags_productid__product_productid");

                    b.HasOne("Druware.Server.Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Article", b =>
                {
                    b.Navigation("ArticleTags");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.AssetType", b =>
                {
                    b.Navigation("Assets");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Document", b =>
                {
                    b.Navigation("DocumentTags");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Product", b =>
                {
                    b.Navigation("History");

                    b.Navigation("ProductTags");
                });
#pragma warning restore 612, 618
        }
    }
}
