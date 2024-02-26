﻿// <auto-generated />
using System;
using Druware.Server.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Druware.Server.Content.Migrations.PostgreSql
{
    [DbContext(typeof(ContentContextPostgreSql))]
    partial class ContentContextPostgreSqlModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Druware.Server.Content.Asset", b =>
                {
                    b.Property<long>("AssetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("asset_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("AssetId"));

                    b.Property<byte[]>("Content")
                        .HasColumnType("bytea")
                        .HasColumnName("content");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("description");

                    b.Property<string>("FileName")
                        .HasMaxLength(192)
                        .HasColumnType("character varying(192)")
                        .HasColumnName("file_name");

                    b.Property<string>("MediaType")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("media_type");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer")
                        .HasColumnName("type_id");

                    b.HasKey("AssetId");

                    b.HasIndex("TypeId");

                    b.ToTable("asset", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Article", b =>
                {
                    b.Property<Guid?>("ArticleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("article_id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uuid")
                        .HasColumnName("author_id");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<string>("ByLine")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("by_line");

                    b.Property<DateTime?>("Expires")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("expires");

                    b.Property<DateTime?>("Modified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Permalink")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("permalink");

                    b.Property<bool>("Pinned")
                        .HasColumnType("boolean")
                        .HasColumnName("pinned");

                    b.Property<DateTime?>("Posted")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("posted")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("summary");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("title");

                    b.HasKey("ArticleId");

                    b.HasIndex("Permalink")
                        .IsUnique();

                    b.ToTable("article", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ArticleTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid?>("ArticleId")
                        .HasColumnType("uuid")
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
                        .HasColumnType("integer")
                        .HasColumnName("type_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int?>("TypeId"));

                    b.Property<string>("Description")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("description");

                    b.HasKey("TypeId")
                        .HasName("asset_type_pkey");

                    b.ToTable("asset_type", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.Document", b =>
                {
                    b.Property<Guid?>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("document_id");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uuid")
                        .HasColumnName("author_id");

                    b.Property<string>("Body")
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<DateTime?>("Modified")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modified");

                    b.Property<string>("Permalink")
                        .HasColumnType("character varying")
                        .HasColumnName("permalink");

                    b.Property<DateTime?>("Posted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("posted")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("title");

                    b.HasKey("DocumentId");

                    b.HasIndex("Permalink")
                        .IsUnique();

                    b.ToTable("document", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.DocumentTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid?>("DocumentId")
                        .HasColumnType("uuid")
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

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("ProductId"));

                    b.Property<DateTime?>("Created")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("DocumentationUrl")
                        .HasMaxLength(278)
                        .HasColumnType("character varying(278)")
                        .HasColumnName("documentation_url");

                    b.Property<string>("DownloadUrl")
                        .HasMaxLength(278)
                        .HasColumnType("character varying(278)")
                        .HasColumnName("download_url");

                    b.Property<string>("IconUrl")
                        .HasMaxLength(278)
                        .HasColumnType("character varying(278)")
                        .HasColumnName("icon_url");

                    b.Property<string>("License")
                        .HasColumnType("text")
                        .HasColumnName("license");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<string>("Short")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("short");

                    b.Property<string>("Summary")
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("summary");

                    b.Property<DateTime?>("Updated")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated")
                        .HasDefaultValueSql("now()");

                    b.HasKey("ProductId");

                    b.HasIndex("Short")
                        .IsUnique();

                    b.ToTable("product", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ProductRelease", b =>
                {
                    b.Property<long?>("ReleaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("release_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("ReleaseId"));

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uuid")
                        .HasColumnName("author_id");

                    b.Property<string>("Body")
                        .HasColumnType("text)")
                        .HasColumnName("body");

                    b.Property<string>("DownloadUrl")
                        .HasMaxLength(278)
                        .HasColumnType("character varying(278)")
                        .HasColumnName("download_url");

                    b.Property<DateTime?>("Modified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modified")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("Posted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("posted")
                        .HasDefaultValueSql("now()");

                    b.Property<long?>("ProductId")
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("title");

                    b.HasKey("ReleaseId");

                    b.HasIndex("ProductId");

                    b.ToTable("product_release", "content");
                });

            modelBuilder.Entity("Druware.Server.Content.Entities.ProductTag", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint");

                    b.Property<long>("TagId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("TagId");

                    b.ToTable("ProductTags");
                });

            modelBuilder.Entity("Druware.Server.Entities.Tag", b =>
                {
                    b.Property<long?>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("tag_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long?>("TagId"));

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("name");

                    b.HasKey("TagId");

                    b.HasIndex("Name")
                        .IsUnique();

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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
