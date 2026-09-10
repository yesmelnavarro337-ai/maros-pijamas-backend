using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExtendBannerWithSeasonAndDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CollectionId",
                table: "Banners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Banners",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SeasonId",
                table: "Banners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Banners",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banners_CollectionId",
                table: "Banners",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_SeasonId",
                table: "Banners",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Collections_CollectionId",
                table: "Banners",
                column: "CollectionId",
                principalTable: "Collections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Seasons_SeasonId",
                table: "Banners",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Collections_CollectionId",
                table: "Banners");

            migrationBuilder.DropForeignKey(
                name: "FK_Banners_Seasons_SeasonId",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_CollectionId",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_SeasonId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Banners");
        }
    }
}
