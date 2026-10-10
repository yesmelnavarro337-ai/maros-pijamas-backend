using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomizationModelAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomizationModelId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CustomizationOptions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CustomizationOptions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomizationOptionAssignments",
                columns: table => new
                {
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationOptionAssignments", x => new { x.ModelId, x.OptionId });
                    table.ForeignKey(
                        name: "FK_CustomizationOptionAssignments_CustomizationOptions_ModelId",
                        column: x => x.ModelId,
                        principalTable: "CustomizationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomizationOptionAssignments_CustomizationOptions_OptionId",
                        column: x => x.OptionId,
                        principalTable: "CustomizationOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CustomizationModelId",
                table: "Products",
                column: "CustomizationModelId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizationOptionAssignments_OptionId",
                table: "CustomizationOptionAssignments",
                column: "OptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CustomizationOptions_CustomizationModelId",
                table: "Products",
                column: "CustomizationModelId",
                principalTable: "CustomizationOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CustomizationOptions_CustomizationModelId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "CustomizationOptionAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Products_CustomizationModelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomizationModelId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CustomizationOptions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CustomizationOptions");
        }
    }
}
