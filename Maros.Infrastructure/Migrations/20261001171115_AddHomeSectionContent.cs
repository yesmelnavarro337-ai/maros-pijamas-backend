using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeSectionContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeSectionContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    SectionTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SectionSubtitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Eyebrow = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    BodyText = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    CtaText = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CtaLink = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MainImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MainImageAlt = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    SecondaryImagesJson = table.Column<string>(type: "text", nullable: true),
                    TagsJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeSectionContents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeSectionContents_SectionKey",
                table: "HomeSectionContents",
                column: "SectionKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeSectionContents");
        }
    }
}
