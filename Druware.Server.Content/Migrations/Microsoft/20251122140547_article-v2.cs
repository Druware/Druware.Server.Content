using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.Microsoft
{
    /// <inheritdoc />
    public partial class articlev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "header_image_id",
                table: "article",
                schema: "content",
                type: "bigint",
                nullable: true);
                
            migrationBuilder.AddColumn<long>(
                name: "icon_id",
                table: "article",
                schema: "content",
                type: "bigint",
                nullable: true);
            
            migrationBuilder.AddColumn<bool>(
                name: "is_featured",
                table: "article",
                schema: "content",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_article_header_image_id",
                schema: "content",
                table: "article",
                column: "header_image_id");

            migrationBuilder.CreateIndex(
                name: "IX_article_icon_id",
                schema: "content",
                table: "article",
                column: "icon_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_article_header_image_id",
                schema: "content",
                table: "article");

            migrationBuilder.DropIndex(
                name: "IX_article_icon_id",
                schema: "content",
                table: "article");
            
            migrationBuilder.DropColumn(
                name: "header_image_id",
                schema: "content",
                table: "article");
            
            migrationBuilder.DropColumn(
                name: "icon_id",
                schema: "content",
                table: "article");
                    
            migrationBuilder.DropColumn(
                name: "is_featured",
                schema: "content",
                table: "article");
        }
    }
}
