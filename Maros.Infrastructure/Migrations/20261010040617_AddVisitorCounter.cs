using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitorCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Valor heredado del antiguo contador en .data/visitors.json (1.335
            // visitas acumuladas). El piso base de instalaciones nuevas sigue
            // siendo 500 (vía SiteSettingsSeed / inicializador de la entidad).
            migrationBuilder.AddColumn<long>(
                name: "TotalVisitors",
                table: "SiteSettings",
                type: "bigint",
                nullable: false,
                defaultValue: 1335L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalVisitors",
                table: "SiteSettings");
        }
    }
}
