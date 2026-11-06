using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class ProductDisableRedirect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_collectionproducts_productid__product_productid",
                schema: "content",
                table: "collection_product");

            migrationBuilder.DropForeignKey(
                name: "fk_productmeta_productid__product_productid",
                schema: "content",
                table: "product_meta");

            migrationBuilder.DropForeignKey(
                name: "fk_content_product_release_product_id__content_product_product_id",
                schema: "content",
                table: "product_release");

            migrationBuilder.DropForeignKey(
                name: "fk_producttags_productid__product_productid",
                schema: "content",
                table: "product_tag");

            migrationBuilder.AddColumn<bool>(
                name: "is_disabled",
                schema: "content",
                table: "product",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "redirect_to",
                schema: "content",
                table: "product",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_collectionproducts_productid__product_productid",
                schema: "content",
                table: "collection_product",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_productmeta_productid__product_productid",
                schema: "content",
                table: "product_meta",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_content_product_release_product_id__content_product_product_id",
                schema: "content",
                table: "product_release",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_producttags_productid__product_productid",
                schema: "content",
                table: "product_tag",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_collectionproducts_productid__product_productid",
                schema: "content",
                table: "collection_product");

            migrationBuilder.DropForeignKey(
                name: "fk_productmeta_productid__product_productid",
                schema: "content",
                table: "product_meta");

            migrationBuilder.DropForeignKey(
                name: "fk_content_product_release_product_id__content_product_product_id",
                schema: "content",
                table: "product_release");

            migrationBuilder.DropForeignKey(
                name: "fk_producttags_productid__product_productid",
                schema: "content",
                table: "product_tag");

            migrationBuilder.DropColumn(
                name: "is_disabled",
                schema: "content",
                table: "product");

            migrationBuilder.DropColumn(
                name: "redirect_to",
                schema: "content",
                table: "product");

            migrationBuilder.AddForeignKey(
                name: "fk_collectionproducts_productid__product_productid",
                schema: "content",
                table: "collection_product",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id");

            migrationBuilder.AddForeignKey(
                name: "fk_productmeta_productid__product_productid",
                schema: "content",
                table: "product_meta",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id");

            migrationBuilder.AddForeignKey(
                name: "fk_content_product_release_product_id__content_product_product_id",
                schema: "content",
                table: "product_release",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id");

            migrationBuilder.AddForeignKey(
                name: "fk_producttags_productid__product_productid",
                schema: "content",
                table: "product_tag",
                column: "product_id",
                principalSchema: "content",
                principalTable: "product",
                principalColumn: "product_id");
        }
    }
}
