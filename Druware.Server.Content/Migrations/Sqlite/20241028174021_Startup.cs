using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.Sqlite
{
    public partial class Startup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "article",
                schema: "content",
                columns: table => new
                {
                    article_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    summary = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    body = table.Column<string>(type: "TEXT", nullable: false),
                    author_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    posted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "date('now')"),
                    modified = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "date('now')"),
                    expires = table.Column<DateTime>(type: "datetime", nullable: true),
                    pinned = table.Column<bool>(type: "INTEGER", nullable: false),
                    permalink = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    by_line = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article", x => x.article_id);
                });

            migrationBuilder.CreateTable(
                name: "asset_type",
                schema: "content",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    description = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("asset_type_pkey", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "document",
                schema: "content",
                columns: table => new
                {
                    document_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    body = table.Column<string>(type: "TEXT", nullable: true),
                    author_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    posted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "date('now')"),
                    modified = table.Column<DateTime>(type: "datetime", nullable: true),
                    permalink = table.Column<string>(type: "varchar(278)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document", x => x.document_id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                schema: "content",
                columns: table => new
                {
                    product_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    @short = table.Column<string>(name: "short", type: "TEXT", maxLength: 32, nullable: true),
                    summary = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    created = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "date('now')"),
                    updated = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "date('now')"),
                    license = table.Column<string>(type: "TEXT", nullable: true),
                    download_url = table.Column<string>(type: "TEXT", maxLength: 278, nullable: true),
                    documentation_url = table.Column<string>(type: "TEXT", maxLength: 278, nullable: true),
                    icon_url = table.Column<string>(type: "TEXT", maxLength: 278, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.product_id);
                });
            

            migrationBuilder.CreateTable(
                name: "asset",
                schema: "content",
                columns: table => new
                {
                    asset_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    content = table.Column<byte[]>(type: "BLOB", nullable: true),
                    media_type = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    file_name = table.Column<string>(type: "TEXT", maxLength: 192, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset", x => x.asset_id);
                    table.ForeignKey(
                        name: "fk_content_asset_type_id__content_asset_type_type_id",
                        column: x => x.type_id,
                        principalSchema: "content",
                        principalTable: "asset_type",
                        principalColumn: "type_id");
                });

            migrationBuilder.CreateTable(
                name: "product_release",
                schema: "content",
                columns: table => new
                {
                    release_id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    product_id = table.Column<long>(type: "INTEGER", nullable: true),
                    title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    body = table.Column<string>(type: "TEXT", nullable: true),
                    author_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    posted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "Date('now')"),
                    modified = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "Date('now')"),
                    download_url = table.Column<string>(type: "TEXT", maxLength: 278, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_release", x => x.release_id);
                    table.ForeignKey(
                        name: "fk_content_product_release_product_id__content_product_product_id",
                        column: x => x.product_id,
                        principalSchema: "content",
                        principalTable: "product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "article_tag",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    article_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    tag_id = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("article_tag_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_article_tag_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_articletags_articleid__article_articleid",
                        column: x => x.article_id,
                        principalSchema: "content",
                        principalTable: "article",
                        principalColumn: "article_id");
                });

            migrationBuilder.CreateTable(
                name: "document_tag",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    document_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    tag_id = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("document_tag_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_document_tag_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_documenttags_documentid__document_documentid",
                        column: x => x.document_id,
                        principalSchema: "content",
                        principalTable: "document",
                        principalColumn: "document_id");
                });

            migrationBuilder.CreateTable(
                name: "product_tag",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    product_id = table.Column<long>(type: "INTEGER", nullable: false),
                    tag_id = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_tag_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_tag_tag_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_producttags_productid__product_productid",
                        column: x => x.product_id,
                        principalSchema: "content",
                        principalTable: "product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_article_permalink",
                schema: "content",
                table: "article",
                column: "permalink",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_article_tag_article_id",
                schema: "content",
                table: "article_tag",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_tag_tag_id",
                schema: "content",
                table: "article_tag",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_asset_type_id",
                schema: "content",
                table: "asset",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_permalink",
                schema: "content",
                table: "document",
                column: "permalink",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_tag_document_id",
                schema: "content",
                table: "document_tag",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_tag_tag_id",
                schema: "content",
                table: "document_tag",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_short",
                schema: "content",
                table: "product",
                column: "short",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_release_product_id",
                schema: "content",
                table: "product_release",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_tag_product_id",
                schema: "content",
                table: "product_tag",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_tag_tag_id",
                schema: "content",
                table: "product_tag",
                column: "tag_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_tag",
                schema: "content");

            migrationBuilder.DropTable(
                name: "asset",
                schema: "content");

            migrationBuilder.DropTable(
                name: "document_tag",
                schema: "content");

            migrationBuilder.DropTable(
                name: "product_release",
                schema: "content");

            migrationBuilder.DropTable(
                name: "product_tag",
                schema: "content");

            migrationBuilder.DropTable(
                name: "article",
                schema: "content");

            migrationBuilder.DropTable(
                name: "asset_type",
                schema: "content");

            migrationBuilder.DropTable(
                name: "document",
                schema: "content");

            migrationBuilder.DropTable(
                name: "product",
                schema: "content");
        }
    }
}
