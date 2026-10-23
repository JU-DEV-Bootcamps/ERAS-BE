using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileFieldsToErasUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "about",
                table: "eras_users",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "eras_users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "employee_id",
                table: "eras_users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "eras_users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "eras_users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "about",
                table: "eras_users");

            migrationBuilder.DropColumn(
                name: "department",
                table: "eras_users");

            migrationBuilder.DropColumn(
                name: "employee_id",
                table: "eras_users");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "eras_users");

            migrationBuilder.DropColumn(
                name: "position",
                table: "eras_users");
        }
    }
}
