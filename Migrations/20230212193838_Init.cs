using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Druware.Server.Content.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "Article",
                schema: "content",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid ()"),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Summary = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Posted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    Expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Pinned = table.Column<bool>(type: "boolean", nullable: false),
                    Permalink = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ByLine = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.ArticleId);
                });

            migrationBuilder.CreateTable(
                name: "asset_type",
                schema: "content",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
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
                    document_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<long>(type: "bigint", nullable: false),
                    posted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    permalink = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document", x => x.document_id);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                schema: "content",
                columns: table => new
                {
                    comment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    article_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    posted = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.comment_id);
                    table.ForeignKey(
                        name: "fk_news_comment_article_id__news_article_article_id",
                        column: x => x.article_id,
                        principalSchema: "content",
                        principalTable: "Article",
                        principalColumn: "ArticleId");
                    table.ForeignKey(
                        name: "fk_news_comment_parent_id__news_comment_comment_id",
                        column: x => x.parent_id,
                        principalSchema: "content",
                        principalTable: "comment",
                        principalColumn: "comment_id");
                });

            migrationBuilder.CreateTable(
                name: "asset",
                schema: "content",
                columns: table => new
                {
                    asset_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<byte[]>(type: "bytea", nullable: true),
                    media_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    file_name = table.Column<string>(type: "character varying(192)", maxLength: 192, nullable: true)
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
                name: "ArticleTag",
                schema: "content",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: true),
                    TagId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("article_tag_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_articletags_articleid__article_articleid",
                        column: x => x.ArticleId,
                        principalSchema: "content",
                        principalTable: "Article",
                        principalColumn: "ArticleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_Permalink",
                schema: "content",
                table: "Article",
                column: "Permalink",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTag_ArticleId",
                schema: "content",
                table: "ArticleTag",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTag_TagId",
                schema: "content",
                table: "ArticleTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_asset_type_id",
                schema: "content",
                table: "asset",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_article_id",
                schema: "content",
                table: "comment",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_parent_id",
                schema: "content",
                table: "comment",
                column: "parent_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTag",
                schema: "content");

            migrationBuilder.DropTable(
                name: "asset",
                schema: "content");

            migrationBuilder.DropTable(
                name: "comment",
                schema: "content");

            migrationBuilder.DropTable(
                name: "document",
                schema: "content");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "asset_type",
                schema: "content");

            migrationBuilder.DropTable(
                name: "Article",
                schema: "content");
        }
    }
}
