using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddInterventionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "student_ids",
                table: "Interventions",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.Sql(
                "UPDATE \"Interventions\" SET student_ids = participant_ids WHERE participant_ids IS NOT NULL");

            migrationBuilder.AddColumn<string>(
                name: "activity",
                table: "Interventions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Interventions\" SET activity = \"ActivityType\"");

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "Interventions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Interventions\" SET remarks = LEFT(\"Comments\", 1000)");

            migrationBuilder.DropColumn(
                name: "ActivityType",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "participant_ids",
                table: "Interventions");

            migrationBuilder.RenameColumn(
                name: "Professional",
                table: "Interventions",
                newName: "professional");

            migrationBuilder.RenameColumn(
                name: "Attachments",
                table: "Interventions",
                newName: "attachments");

            migrationBuilder.AlterColumn<string>(
                name: "professional",
                table: "Interventions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "attendance",
                table: "Interventions",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "mode",
                table: "Interventions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "number_of_guests",
                table: "Interventions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_of_participants",
                table: "Interventions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "Interventions",
                type: "text",
                nullable: false,
                defaultValue: "Created");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityType",
                table: "Interventions",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Interventions\" SET \"ActivityType\" = activity");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Interventions",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Interventions\" SET \"Comments\" = remarks");

            migrationBuilder.AddColumn<int[]>(
                name: "participant_ids",
                table: "Interventions",
                type: "integer[]",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Interventions\" SET participant_ids = student_ids");

            migrationBuilder.DropColumn(
                name: "activity",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "attendance",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "mode",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "number_of_guests",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "number_of_participants",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Interventions");

            migrationBuilder.DropColumn(
                name: "student_ids",
                table: "Interventions");

            migrationBuilder.RenameColumn(
                name: "professional",
                table: "Interventions",
                newName: "Professional");

            migrationBuilder.RenameColumn(
                name: "attachments",
                table: "Interventions",
                newName: "Attachments");

            migrationBuilder.AlterColumn<string>(
                name: "Professional",
                table: "Interventions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}