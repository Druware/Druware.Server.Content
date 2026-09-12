using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class ProductPlatformLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "direct_gnome",
                schema: "content",
                table: "product",
                type: "character varying(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "direct_haiku",
                schema: "content",
                table: "product",
                type: "character varying(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "direct_kde",
                schema: "content",
                table: "product",
                type: "character varying(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "repo_github",
                schema: "content",
                table: "product",
                type: "character varying(278)",
                maxLength: 278,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "direct_gnome",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "direct_haiku",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "direct_kde",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "repo_github",
                schema: "content",
                table: "product");
        }
    }
}
