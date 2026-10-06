using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductStylesManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Styles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    HexCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Styles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductStyles",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    StyleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStyles", x => new { x.ProductId, x.StyleId });
                    table.ForeignKey(
                        name: "FK_ProductStyles_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductStyles_Styles_StyleId",
                        column: x => x.StyleId,
                        principalTable: "Styles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductStyles_StyleId",
                table: "ProductStyles",
                column: "StyleId");

            migrationBuilder.CreateIndex(
                name: "IX_Styles_Slug",
                table: "Styles",
                column: "Slug",
                unique: true);

            // Catálogo cerrado de estilos oficiales (9 modelos) usado historically por maros-admin
            // mediante la constante FIXED_STYLES. Los Guid son fijos para que el seed sea idempotente.
            migrationBuilder.Sql("""
                INSERT INTO "Styles" ("Id", "Name", "Slug", "HexCode", "DisplayOrder", "IsActive", "CreatedAt") VALUES
                    ('11111111-1111-1111-1111-111111111101', 'Short - Camisa manga corta (Mujer)',  'short-camisa-manga-corta-mujer',    NULL, 1, true, now()),
                    ('11111111-1111-1111-1111-111111111102', 'Short - Camisa manga corta (Hombre)', 'short-camisa-manga-corta-hombre',   NULL, 2, true, now()),
                    ('11111111-1111-1111-1111-111111111103', 'Batas',                                'batas',                              NULL, 3, true, now()),
                    ('11111111-1111-1111-1111-111111111104', 'Pantalón - Camisa manga corta (Mujer)',  'pantalon-camisa-manga-corta-mujer',  NULL, 4, true, now()),
                    ('11111111-1111-1111-1111-111111111105', 'Pantalón - Camisa manga corta (Hombre)', 'pantalon-camisa-manga-corta-hombre', NULL, 5, true, now()),
                    ('11111111-1111-1111-1111-111111111106', 'Pantalón - Camisa manga larga (Mujer)',  'pantalon-camisa-manga-larga-mujer',  NULL, 6, true, now()),
                    ('11111111-1111-1111-1111-111111111107', 'Pantalón - Camisa manga larga (Hombre)', 'pantalon-camisa-manga-larga-hombre', NULL, 7, true, now()),
                    ('11111111-1111-1111-1111-111111111108', 'Short - Camiseta (Hombre / Mujer)',      'short-camiseta-hombre-mujer',        NULL, 8, true, now()),
                    ('11111111-1111-1111-1111-111111111109', 'Pantalón - Camiseta (Hombre / Mujer)',   'pantalon-camiseta-hombre-mujer',     NULL, 9, true, now())
                ON CONFLICT ("Id") DO NOTHING;
                """);

            // Backfill: los estilos ya guardados en variantes pasan a la relación canónica.
            migrationBuilder.Sql("""
                INSERT INTO "ProductStyles" ("ProductId", "StyleId")
                SELECT DISTINCT v."ProductId", s."Id"
                FROM "ProductVariants" v
                INNER JOIN "Styles" s ON lower(btrim(s."Name")) = lower(btrim(v."StyleName"))
                WHERE v."StyleName" IS NOT NULL AND btrim(v."StyleName") <> ''
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductStyles");

            migrationBuilder.DropTable(
                name: "Styles");
        }
    }
}
