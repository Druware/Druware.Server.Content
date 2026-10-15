using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ArticleTakesAndMeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "dru_take",
                schema: "content",
                table: "article",
                type: "TEXT",
                maxLength: 280,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "eddie_take",
                schema: "content",
                table: "article",
                type: "TEXT",
                maxLength: 280,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "article_meta",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    article_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    property = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    value = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("article_meta_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_articlemeta_articleid__article_articleid",
                        column: x => x.article_id,
                        principalSchema: "content",
                        principalTable: "article",
                        principalColumn: "article_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_article_meta_article_id_property",
                schema: "content",
                table: "article_meta",
                columns: new[] { "article_id", "property" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_meta",
                schema: "content");

            migrationBuilder.DropColumn(
                name: "dru_take",
                schema: "content",
                table: "article");

            migrationBuilder.DropColumn(
                name: "eddie_take",
                schema: "content",
                table: "article");
        }
    }
}
