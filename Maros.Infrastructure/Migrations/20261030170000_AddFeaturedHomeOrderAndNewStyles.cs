using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedHomeOrderAndNewStyles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeaturedHomeOrder",
                table: "Products",
                type: "integer",
                nullable: true);

            // 1. Sembrar los 3 nuevos estilos oficiales si aún no existen
            migrationBuilder.Sql("""
                INSERT INTO "Styles" ("Id", "Name", "Slug", "HexCode", "DisplayOrder", "IsActive", "Line", "CreatedAt")
                VALUES
                    ('11111111-1111-1111-1111-111111111110', 'Short - Tiras (Mujer)', 'short-tiras-mujer', NULL, 10, TRUE, 0, NOW()),
                    ('11111111-1111-1111-1111-111111111204', 'Short - Camiseta (Infantil)', 'short-camiseta-infantil', NULL, 11, TRUE, 1, NOW()),
                    ('11111111-1111-1111-1111-111111111205', 'Pantalón - Camiseta (Infantil)', 'pantalon-camiseta-infantil', NULL, 12, TRUE, 1, NOW())
                ON CONFLICT ("Id") DO NOTHING;
                """);

            // 2. Asignar los nuevos estilos a los productos activos si no los tienen registrados
            // Adulto: Short - Tiras (Mujer) a productos activos que no sean exclusivamente infantiles
            migrationBuilder.Sql("""
                INSERT INTO "ProductStyles" ("ProductId", "StyleId")
                SELECT p."Id", '11111111-1111-1111-1111-111111111110'
                FROM "Products" p
                WHERE p."IsDeleted" = FALSE AND p."Status" = 'Activo'
                  AND NOT EXISTS (
                      SELECT 1 FROM "ProductStyles" ps 
                      WHERE ps."ProductId" = p."Id" AND ps."StyleId" = '11111111-1111-1111-1111-111111111110'
                  )
                ON CONFLICT DO NOTHING;
                """);

            // Infantil: Short - Camiseta (Infantil) y Pantalón - Camiseta (Infantil) a productos activos
            migrationBuilder.Sql("""
                INSERT INTO "ProductStyles" ("ProductId", "StyleId")
                SELECT p."Id", '11111111-1111-1111-1111-111111111204'
                FROM "Products" p
                WHERE p."IsDeleted" = FALSE AND p."Status" = 'Activo'
                  AND NOT EXISTS (
                      SELECT 1 FROM "ProductStyles" ps 
                      WHERE ps."ProductId" = p."Id" AND ps."StyleId" = '11111111-1111-1111-1111-111111111204'
                  )
                ON CONFLICT DO NOTHING;

                INSERT INTO "ProductStyles" ("ProductId", "StyleId")
                SELECT p."Id", '11111111-1111-1111-1111-111111111205'
                FROM "Products" p
                WHERE p."IsDeleted" = FALSE AND p."Status" = 'Activo'
                  AND NOT EXISTS (
                      SELECT 1 FROM "ProductStyles" ps 
                      WHERE ps."ProductId" = p."Id" AND ps."StyleId" = '11111111-1111-1111-1111-111111111205'
                  )
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "ProductStyles"
                WHERE "StyleId" IN (
                    '11111111-1111-1111-1111-111111111110',
                    '11111111-1111-1111-1111-111111111204',
                    '11111111-1111-1111-1111-111111111205'
                );
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Styles"
                WHERE "Id" IN (
                    '11111111-1111-1111-1111-111111111110',
                    '11111111-1111-1111-1111-111111111204',
                    '11111111-1111-1111-1111-111111111205'
                );
                """);

            migrationBuilder.DropColumn(
                name: "FeaturedHomeOrder",
                table: "Products");
        }
    }
}

