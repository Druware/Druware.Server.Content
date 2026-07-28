using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.Microsoft
{
    /// <inheritdoc />
    public partial class ProductMeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product_meta",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    property = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    value = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_meta_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_productmeta_productid__product_productid",
                        column: x => x.product_id,
                        principalSchema: "content",
                        principalTable: "product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_meta_product_id_property",
                schema: "content",
                table: "product_meta",
                columns: new[] { "product_id", "property" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_meta",
                schema: "content");
        }
    }
}
