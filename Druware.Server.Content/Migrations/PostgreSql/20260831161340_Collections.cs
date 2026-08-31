using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Druware.Server.Content.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class Collections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "collection",
                schema: "content",
                columns: table => new
                {
                    collection_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collection", x => x.collection_id);
                });

            migrationBuilder.CreateTable(
                name: "collection_product",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    collection_id = table.Column<long>(type: "bigint", nullable: false),
                    product_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("collection_product_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_collectionproducts_collectionid__collection_collectionid",
                        column: x => x.collection_id,
                        principalSchema: "content",
                        principalTable: "collection",
                        principalColumn: "collection_id");
                    table.ForeignKey(
                        name: "fk_collectionproducts_productid__product_productid",
                        column: x => x.product_id,
                        principalSchema: "content",
                        principalTable: "product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_collection_name",
                schema: "content",
                table: "collection",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_collection_product_collection_id_product_id",
                schema: "content",
                table: "collection_product",
                columns: new[] { "collection_id", "product_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_collection_product_product_id",
                schema: "content",
                table: "collection_product",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collection_product",
                schema: "content");

            migrationBuilder.DropTable(
                name: "collection",
                schema: "content");
        }
    }
}
