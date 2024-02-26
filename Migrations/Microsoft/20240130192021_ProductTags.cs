using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.Microsoft
{
    public partial class ProductTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_tag",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
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
                name: "product_tag",
                schema: "content");
        }
    }
}
