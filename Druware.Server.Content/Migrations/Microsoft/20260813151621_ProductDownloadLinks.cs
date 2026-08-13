using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.Microsoft
{
    /// <inheritdoc />
    public partial class ProductDownloadLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "app_store_amazon",
                schema: "content",
                table: "product",
                type: "nvarchar(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "app_store_apple",
                schema: "content",
                table: "product",
                type: "nvarchar(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "app_store_google",
                schema: "content",
                table: "product",
                type: "nvarchar(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "app_store_ms",
                schema: "content",
                table: "product",
                type: "nvarchar(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "direct_osx",
                schema: "content",
                table: "product",
                type: "nvarchar(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "direct_win_arm",
                schema: "content",
                table: "product",
                type: "nvarchar(278)",
                maxLength: 278,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "direct_win_x64",
                schema: "content",
                table: "product",
                type: "nvarchar(278)",
                maxLength: 278,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "app_store_amazon",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "app_store_apple",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "app_store_google",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "app_store_ms",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "direct_osx",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "direct_win_arm",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "direct_win_x64",
                schema: "content",
                table: "product");
        }
    }
}
