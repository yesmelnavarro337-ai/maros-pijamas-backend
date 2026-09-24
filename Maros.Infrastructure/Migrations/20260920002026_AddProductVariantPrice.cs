using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVariantPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ProductVariants",
                type: "numeric(12,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ProductVariants");
        }
    }
}
