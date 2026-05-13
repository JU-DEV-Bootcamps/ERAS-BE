using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    public partial class fixuuids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "new_participant_ids",
                table: "Interventions",
                type: "integer[]",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""Interventions"" i
                SET new_participant_ids = (
                    SELECT ARRAY_AGG(s.""Id"" ORDER BY array_position(i.""participant_ids""::text[], s.""uuid""::text))
                    FROM ""students"" s
                    WHERE s.""uuid""::text = ANY(i.""participant_ids""::text[])
                );
            ");

            migrationBuilder.DropColumn(
                name: "participant_ids",
                table: "Interventions");
            
            migrationBuilder.RenameColumn(
                name: "new_participant_ids",
                table: "Interventions",
                newName: "participant_ids"
            );

           migrationBuilder.AddColumn<int[]>(
                name: "new_student_ids",
                table: "remissions",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[] { });

            migrationBuilder.Sql(@"
                UPDATE ""remissions"" r
                SET new_student_ids = (
                    SELECT ARRAY_AGG(s.""Id"" ORDER BY array_position(r.""student_ids""::text[], s.""uuid""::text))
                    FROM ""students"" s
                    WHERE s.""uuid""::text = ANY(r.""student_ids""::text[])
                );
            ");

            migrationBuilder.DropColumn(
                name: "student_ids",
                table: "remissions");

            migrationBuilder.RenameColumn(
                name: "new_student_ids",
                table: "remissions",
                newName: "student_ids");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid[]>(
                name: "new_participant_ids",
                table: "Interventions",
                type: "uuid[]",
                nullable: true,
                defaultValue: Array.Empty<Guid>()
            );

            migrationBuilder.Sql(@"
                UPDATE ""Interventions"" i
                SET new_participant_ids = (
                    SELECT ARRAY_AGG(s.""uuid"" ORDER BY array_position(i.""participant_ids"", s.""Id""))::uuid[]
                    FROM ""students"" s
                    WHERE s.""Id"" = ANY(i.""participant_ids"")
                );
            ");

            migrationBuilder.DropColumn(
                name: "participant_ids",
                table: "Interventions");

            migrationBuilder.RenameColumn(
                name: "new_participant_ids",
                table: "Interventions",
                newName: "participant_ids"
            );

            migrationBuilder.AddColumn<Guid[]>(
                name: "new_student_ids",
                table: "remissions",
                type: "uuid[]",
                nullable: false,
                defaultValue: Array.Empty<Guid>()
            );

            migrationBuilder.Sql(@"
                UPDATE ""remissions"" r
                SET new_student_ids = (
                    SELECT ARRAY_AGG(s.""uuid"" ORDER BY array_position(r.""student_ids"", s.""Id""))::uuid[]
                    FROM ""students"" s
                    WHERE s.""Id"" = ANY(r.""student_ids"")
                );
            ");

            migrationBuilder.DropColumn(
                name: "student_ids",
                table: "remissions");

            migrationBuilder.RenameColumn(
                name: "new_student_ids",
                table: "remissions",
                newName: "student_ids");
        }
    }
}