using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class entities_versioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "version",
                table: "polls");

            migrationBuilder.AddColumn<int>(
                name: "last_version",
                table: "polls",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_version_date",
                table: "polls",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "version_date",
                table: "poll_variable",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "version_number",
                table: "poll_variable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "last_version",
                table: "poll_instances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_version_date",
                table: "poll_instances",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "version_date",
                table: "answers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "version_number",
                table: "answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_version",
                table: "polls");

            migrationBuilder.DropColumn(
                name: "last_version_date",
                table: "polls");

            migrationBuilder.DropColumn(
                name: "version_date",
                table: "poll_variable");

            migrationBuilder.DropColumn(
                name: "version_number",
                table: "poll_variable");

            migrationBuilder.DropColumn(
                name: "last_version",
                table: "poll_instances");

            migrationBuilder.DropColumn(
                name: "last_version_date",
                table: "poll_instances");

            migrationBuilder.DropColumn(
                name: "version_date",
                table: "answers");

            migrationBuilder.DropColumn(
                name: "version_number",
                table: "answers");

            migrationBuilder.AddColumn<string>(
                name: "version",
                table: "polls",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
