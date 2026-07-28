using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class ProductTagMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTags_product_ProductId",
                table: "ProductTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTags_tag_TagId",
                table: "ProductTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTags",
                table: "ProductTags");

            migrationBuilder.RenameTable(
                name: "ProductTags",
                newName: "product_tag",
                newSchema: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "content",
                table: "product_tag",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TagId",
                schema: "content",
                table: "product_tag",
                newName: "tag_id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                schema: "content",
                table: "product_tag",
                newName: "product_id");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTags_TagId",
                schema: "content",
                table: "product_tag",
                newName: "IX_product_tag_tag_id");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTags_ProductId",
                schema: "content",
                table: "product_tag",
                newName: "IX_product_tag_product_id");

            migrationBuilder.AddPrimaryKey(
                name: "product_tag_pkey",
                schema: "content",
                table: "product_tag",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_tag_tag_tag_id",
                schema: "content",
                table: "product_tag",
                column: "tag_id",
                principalTable: "tag",
                principalColumn: "tag_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_producttags_productid__product_productid",
                schema: "content",
                table: "product_tag",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_tag_tag_tag_id",
                schema: "content",
                table: "product_tag");

            migrationBuilder.DropForeignKey(
                name: "fk_producttags_productid__product_productid",
                schema: "content",
                table: "product_tag");

            migrationBuilder.DropPrimaryKey(
                name: "product_tag_pkey",
                schema: "content",
                table: "product_tag");

            migrationBuilder.RenameTable(
                name: "product_tag",
                schema: "content",
                newName: "ProductTags");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ProductTags",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tag_id",
                table: "ProductTags",
                newName: "TagId");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "ProductTags",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_product_tag_tag_id",
                table: "ProductTags",
                newName: "IX_ProductTags_TagId");

            migrationBuilder.RenameIndex(
                name: "IX_product_tag_product_id",
                table: "ProductTags",
                newName: "IX_ProductTags_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTags",
                table: "ProductTags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTags_product_ProductId",
                table: "ProductTags",
                column: "ProductId",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTags_tag_TagId",
                table: "ProductTags",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "tag_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
