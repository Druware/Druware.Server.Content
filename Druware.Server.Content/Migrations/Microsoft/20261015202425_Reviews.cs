using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Druware.Server.Content.Migrations.Microsoft
{
    /// <inheritdoc />
    public partial class Reviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "review",
                schema: "content",
                columns: table => new
                {
                    review_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    permalink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    product_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    category = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    summary = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    photo_asset_id = table.Column<long>(type: "bigint", nullable: true),
                    product_url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    is_affiliate = table.Column<bool>(type: "bit", nullable: false),
                    eddie_score = table.Column<int>(type: "int", nullable: true),
                    dru_score = table.Column<int>(type: "int", nullable: true),
                    eddie_take = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true),
                    dru_take = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: true),
                    posted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_published = table.Column<bool>(type: "bit", nullable: false),
                    is_featured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("review_pkey", x => x.review_id);
                    table.ForeignKey(
                        name: "fk_review_photoassetid__asset_assetid",
                        column: x => x.photo_asset_id,
                        principalSchema: "content",
                        principalTable: "asset",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "review_tag",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    review_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("review_tag_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_reviewtags_reviewid__review_reviewid",
                        column: x => x.review_id,
                        principalSchema: "content",
                        principalTable: "review",
                        principalColumn: "review_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reviewtags_tagid__tag_tagid",
                        column: x => x.tag_id,
                        principalTable: "tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_review_permalink",
                schema: "content",
                table: "review",
                column: "permalink",
                unique: true,
                filter: "[permalink] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_review_photo_asset_id",
                schema: "content",
                table: "review",
                column: "photo_asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_tag_review_id",
                schema: "content",
                table: "review_tag",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_tag_tag_id",
                schema: "content",
                table: "review_tag",
                column: "tag_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "review_tag",
                schema: "content");

            migrationBuilder.DropTable(
                name: "review",
                schema: "content");
        }
    }
}
