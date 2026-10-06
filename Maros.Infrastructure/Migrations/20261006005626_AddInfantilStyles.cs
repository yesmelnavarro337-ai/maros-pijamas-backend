using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Añade la línea de producto a los estilos y siembra el catálogo infantil canónico.
    /// Ya está aplicada en la base de datos existente: EF la omite por su MigrationId.
    /// </remarks>
    public partial class AddInfantilStyles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 0 = Adulto (enum StyleLine), así que los estilos preexistentes no
            // necesitan backfill.
            migrationBuilder.AddColumn<int>(
                name: "Line",
                table: "Styles",
                nullable: false,
                defaultValue: 0);

            // Idempotente: los GUID fijos y ON CONFLICT evitan duplicar el catálogo
            // si la migración se reintenta sobre una base ya sembrada.
            migrationBuilder.Sql("""
                INSERT INTO "Styles" ("Id", "Name", "Slug", "HexCode", "DisplayOrder", "IsActive", "Line", "CreatedAt")
                VALUES
                    ('11111111-1111-1111-1111-111111111201', 'Short - Camisa manga corta (Infantil)', 'short---camisa-manga-corta-infantil', NULL, 0, TRUE, 1, NOW()),
                    ('11111111-1111-1111-1111-111111111202', 'Pantalón - Camisa manga corta (Infantil)', 'pantalon---camisa-manga-corta-infantil', NULL, 0, TRUE, 1, NOW()),
                    ('11111111-1111-1111-1111-111111111203', 'Pantalón - Camisa manga larga (Infantil)', 'pantalon---camisa-manga-larga-infantil', NULL, 0, TRUE, 1, NOW())
                ON CONFLICT DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "Styles"
                WHERE "Id" IN (
                    '11111111-1111-1111-1111-111111111201',
                    '11111111-1111-1111-1111-111111111202',
                    '11111111-1111-1111-1111-111111111203'
                );
                """);

            migrationBuilder.DropColumn(
                name: "Line",
                table: "Styles");
        }
    }
}