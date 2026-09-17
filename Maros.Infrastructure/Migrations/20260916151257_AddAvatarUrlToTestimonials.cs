using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maros.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToTestimonials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Testimonials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Testimonials",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Testimonials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AutoReplyMessage",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BackupRetentionDays",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BackupTime",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CanonicalUrl",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateFormat",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultSubject",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastBackupSize",
                table: "SiteSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalCookiesPolicy",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegalPrivacyPolicy",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegalTermsAndConditions",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LockoutDurationMinutes",
                table: "SiteSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MapImageUrl",
                table: "SiteSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxLoginAttempts",
                table: "SiteSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RobotsTag",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SecurityNotificationsEnabled",
                table: "SiteSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ServerIp",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShowLocation",
                table: "SiteSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "SiteSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Whatsapp",
                table: "SiteSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WhatsappButtonEnabled",
                table: "SiteSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WhatsappButtonImageUrl",
                table: "SiteSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsappPosition",
                table: "SiteSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WwwRedirect",
                table: "SiteSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Youtube",
                table: "SiteSettings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "AutoReplyMessage",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "BackupRetentionDays",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "BackupTime",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "CanonicalUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "DateFormat",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "DefaultSubject",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "LastBackupSize",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "LegalCookiesPolicy",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "LegalPrivacyPolicy",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "LegalTermsAndConditions",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "LockoutDurationMinutes",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "MapImageUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "MaxLoginAttempts",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "RobotsTag",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "SecurityNotificationsEnabled",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ServerIp",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ShowLocation",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "Whatsapp",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhatsappButtonEnabled",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhatsappButtonImageUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WhatsappPosition",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "WwwRedirect",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "Youtube",
                table: "SiteSettings");
        }
    }
}
