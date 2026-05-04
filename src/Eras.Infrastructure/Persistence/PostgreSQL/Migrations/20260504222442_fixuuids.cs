using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    public partial class fixuuids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "participant_ids",
                table: "Interventions");

            migrationBuilder.AddColumn<int[]>(
                name: "participant_ids",
                table: "Interventions",
                type: "integer[]",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "student_ids",
                table: "remissions");

            migrationBuilder.AddColumn<int[]>(
                name: "student_ids",
                table: "remissions",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[] { });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}