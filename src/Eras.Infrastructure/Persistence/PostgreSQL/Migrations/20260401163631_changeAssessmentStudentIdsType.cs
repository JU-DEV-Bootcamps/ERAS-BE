using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    public partial class changeAssessmentStudentIdsType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid[]>(
                name: "student_ids_new",
                table: "remissions",
                type: "uuid[]",
                nullable: false,
                defaultValue: Array.Empty<Guid>());

            migrationBuilder.Sql("""
                UPDATE remissions
                SET student_ids_new =
                    COALESCE(
                        ARRAY(
                            SELECT jsonb_array_elements_text(student_ids)::uuid
                        ),
                        ARRAY[]::uuid[]
                    );
            """);

            migrationBuilder.DropColumn(
                name: "student_ids",
                table: "remissions");

            migrationBuilder.RenameColumn(
                name: "student_ids_new",
                table: "remissions",
                newName: "student_ids");

            migrationBuilder.AddColumn<Guid[]>(
                name: "participant_ids_new",
                table: "Interventions",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Interventions"
                SET participant_ids_new =
                    CASE
                        WHEN participant_ids IS NULL THEN NULL
                        ELSE ARRAY(
                            SELECT jsonb_array_elements_text(participant_ids)::uuid
                        )
                    END;
            """);

            migrationBuilder.DropColumn(
                name: "participant_ids",
                table: "Interventions");

            migrationBuilder.RenameColumn(
                name: "participant_ids_new",
                table: "Interventions",
                newName: "participant_ids");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "student_ids_old",
                table: "remissions",
                type: "jsonb",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.Sql("""
                UPDATE remissions
                SET student_ids_old = to_jsonb(student_ids)::text;
            """);

            migrationBuilder.DropColumn(
                name: "student_ids",
                table: "remissions");

            migrationBuilder.RenameColumn(
                name: "student_ids_old",
                table: "remissions",
                newName: "student_ids");

            migrationBuilder.AddColumn<string>(
                name: "participant_ids_old",
                table: "Interventions",
                type: "jsonb",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Interventions"
                SET participant_ids_old =
                    CASE
                        WHEN participant_ids IS NULL THEN NULL
                        ELSE to_jsonb(participant_ids)::text
                    END;
            """);

            migrationBuilder.DropColumn(
                name: "participant_ids",
                table: "Interventions");

            migrationBuilder.RenameColumn(
                name: "participant_ids_old",
                table: "Interventions",
                newName: "participant_ids");
        }
    }
}