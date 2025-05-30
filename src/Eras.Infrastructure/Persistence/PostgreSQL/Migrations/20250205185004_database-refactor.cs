using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class DatabaseRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.CreateTable(
                name: "components",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = Table.Column<string>(type: "text", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_components", X => X.Id);
                });

            MigrationBuilder.CreateTable(
                name: "polls",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = Table.Column<string>(type: "text", nullable: false),
                    version = Table.Column<string>(type: "text", nullable: false),
                    uuid = Table.Column<string>(type: "text", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_polls", X => X.Id);
                });

            MigrationBuilder.CreateTable(
                name: "students",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = Table.Column<string>(type: "text", nullable: false),
                    name = Table.Column<string>(type: "text", nullable: false),
                    email = Table.Column<string>(type: "text", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_students", X => X.Id);
                });

            MigrationBuilder.CreateTable(
                name: "variables",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = Table.Column<string>(type: "text", nullable: false),
                    ComponentId = Table.Column<int>(type: "integer", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_variables", X => X.Id);
                    Table.ForeignKey(
                        name: "FK_variables_components_ComponentId",
                        column: X => X.ComponentId,
                        principalTable: "components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateTable(
                name: "poll_instances",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = Table.Column<string>(type: "text", nullable: false),
                    StudentId = Table.Column<int>(type: "integer", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_poll_instances", X => X.Id);
                    Table.ForeignKey(
                        name: "FK_poll_instances_students_StudentId",
                        column: X => X.StudentId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateTable(
                name: "student_details",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    enrolled_courses = Table.Column<int>(type: "integer", nullable: false),
                    graded_courses = Table.Column<int>(type: "integer", nullable: false),
                    time_delivery_rate = Table.Column<int>(type: "integer", nullable: false),
                    avg_score = Table.Column<decimal>(type: "numeric", nullable: false),
                    courses_under_avg = Table.Column<decimal>(type: "numeric", nullable: false),
                    pure_score_diff = Table.Column<decimal>(type: "numeric", nullable: false),
                    standard_score_diff = Table.Column<decimal>(type: "numeric", nullable: false),
                    last_access_days = Table.Column<int>(type: "integer", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StudentId = Table.Column<int>(type: "integer", nullable: false)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_student_details", X => X.Id);
                    Table.ForeignKey(
                        name: "FK_student_details_students_StudentId",
                        column: X => X.StudentId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateTable(
                name: "cohorts",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = Table.Column<string>(type: "text", nullable: false),
                    course_name = Table.Column<string>(type: "text", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VariableId = Table.Column<int>(type: "integer", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_cohorts", X => X.Id);
                    Table.ForeignKey(
                        name: "FK_cohorts_variables_VariableId",
                        column: X => X.VariableId,
                        principalTable: "variables",
                        principalColumn: "Id");
                });

            MigrationBuilder.CreateTable(
                name: "poll_variable",
                columns: Table => new
                {
                    poll_id = Table.Column<int>(type: "integer", nullable: false),
                    variable_id = Table.Column<int>(type: "integer", nullable: false)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_poll_variable", X => new { X.poll_id, X.variable_id });
                    Table.ForeignKey(
                        name: "FK_poll_variable_polls_poll_id",
                        column: X => X.poll_id,
                        principalTable: "polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    Table.ForeignKey(
                        name: "FK_poll_variable_variables_variable_id",
                        column: X => X.variable_id,
                        principalTable: "variables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateTable(
                name: "answers",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    answer_text = Table.Column<string>(type: "text", nullable: false),
                    risk_level = Table.Column<int>(type: "integer", nullable: false),
                    PollInstanceId = Table.Column<int>(type: "integer", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_answers", X => X.Id);
                    Table.ForeignKey(
                        name: "FK_answers_poll_instances_PollInstanceId",
                        column: X => X.PollInstanceId,
                        principalTable: "poll_instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateTable(
                name: "student_cohort",
                columns: Table => new
                {
                    student_id = Table.Column<int>(type: "integer", nullable: false),
                    cohort_id = Table.Column<int>(type: "integer", nullable: false)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_student_cohort", X => new { X.student_id, X.cohort_id });
                    Table.ForeignKey(
                        name: "FK_student_cohort_cohorts_cohort_id",
                        column: X => X.cohort_id,
                        principalTable: "cohorts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    Table.ForeignKey(
                        name: "FK_student_cohort_students_student_id",
                        column: X => X.student_id,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateIndex(
                name: "IX_answers_PollInstanceId",
                table: "answers",
                column: "PollInstanceId");

            MigrationBuilder.CreateIndex(
                name: "IX_cohorts_VariableId",
                table: "cohorts",
                column: "VariableId");

            MigrationBuilder.CreateIndex(
                name: "IX_poll_instances_StudentId",
                table: "poll_instances",
                column: "StudentId");

            MigrationBuilder.CreateIndex(
                name: "IX_poll_variable_variable_id",
                table: "poll_variable",
                column: "variable_id");

            MigrationBuilder.CreateIndex(
                name: "IX_student_cohort_cohort_id",
                table: "student_cohort",
                column: "cohort_id");

            MigrationBuilder.CreateIndex(
                name: "IX_student_details_StudentId",
                table: "student_details",
                column: "StudentId",
                unique: true);

            MigrationBuilder.CreateIndex(
                name: "IX_variables_ComponentId",
                table: "variables",
                column: "ComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropTable(
                name: "answers");

            MigrationBuilder.DropTable(
                name: "poll_variable");

            MigrationBuilder.DropTable(
                name: "student_cohort");

            MigrationBuilder.DropTable(
                name: "student_details");

            MigrationBuilder.DropTable(
                name: "poll_instances");

            MigrationBuilder.DropTable(
                name: "polls");

            MigrationBuilder.DropTable(
                name: "cohorts");

            MigrationBuilder.DropTable(
                name: "students");

            MigrationBuilder.DropTable(
                name: "variables");

            MigrationBuilder.DropTable(
                name: "components");
        }
    }
}
