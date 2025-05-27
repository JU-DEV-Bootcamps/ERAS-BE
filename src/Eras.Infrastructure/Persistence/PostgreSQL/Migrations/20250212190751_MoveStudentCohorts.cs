using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class MoveStudentCohorts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort");

            MigrationBuilder.AddColumn<int>(
                name: "Id",
                table: "student_cohort",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            MigrationBuilder.AddPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort",
                column: "Id");

            MigrationBuilder.CreateTable(
                name: "CohortEntityStudentEntity",
                columns: Table => new
                {
                    CohortsId = Table.Column<int>(type: "integer", nullable: false),
                    StudentsId = Table.Column<int>(type: "integer", nullable: false)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_CohortEntityStudentEntity", X => new { X.CohortsId, X.StudentsId });
                    Table.ForeignKey(
                        name: "FK_CohortEntityStudentEntity_cohorts_CohortsId",
                        column: X => X.CohortsId,
                        principalTable: "cohorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    Table.ForeignKey(
                        name: "FK_CohortEntityStudentEntity_students_StudentsId",
                        column: X => X.StudentsId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateIndex(
                name: "IX_student_cohort_student_id",
                table: "student_cohort",
                column: "student_id");

            MigrationBuilder.CreateIndex(
                name: "IX_CohortEntityStudentEntity_StudentsId",
                table: "CohortEntityStudentEntity",
                column: "StudentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropTable(
                name: "CohortEntityStudentEntity");

            MigrationBuilder.DropPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort");

            MigrationBuilder.DropIndex(
                name: "IX_student_cohort_student_id",
                table: "student_cohort");

            MigrationBuilder.DropColumn(
                name: "Id",
                table: "student_cohort");

            MigrationBuilder.AddPrimaryKey(
                name: "PK_student_cohort",
                table: "student_cohort",
                columns: new[] { "student_id", "cohort_id" });
        }
    }
}
