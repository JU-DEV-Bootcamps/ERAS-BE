using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class MoveStudentCohorts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "student_cohort",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CohortEntityStudentEntity",
                columns: table => new
                {
                    CohortsId = table.Column<int>(type: "integer", nullable: false),
                    StudentsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CohortEntityStudentEntity", x => new { x.CohortsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_CohortEntityStudentEntity_cohorts_CohortsId",
                        column: x => x.CohortsId,
                        principalTable: "cohorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CohortEntityStudentEntity_students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_cohort_student_id",
                table: "student_cohort",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_CohortEntityStudentEntity_StudentsId",
                table: "CohortEntityStudentEntity",
                column: "StudentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CohortEntityStudentEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort");

            migrationBuilder.DropIndex(
                name: "IX_student_cohort_student_id",
                table: "student_cohort");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "student_cohort");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort",
                columns: new[] { "student_id", "cohort_id" });
        }
    }
}
