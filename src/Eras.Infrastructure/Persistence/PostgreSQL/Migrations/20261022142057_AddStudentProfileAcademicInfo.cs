using System.Collections.Generic;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProfileAcademicInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "cgpa",
                table: "student_profiles",
                type: "numeric(4,2)",
                precision: 4,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "current_courses",
                table: "student_profiles",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "failed_courses_count",
                table: "student_profiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "identified_risks",
                table: "student_profiles",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_current_course_third_attempt",
                table: "student_profiles",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "passed_courses_count",
                table: "student_profiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sap_status",
                table: "student_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "stu_id",
                table: "student_profiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "support_received",
                table: "student_profiles",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cgpa",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "current_courses",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "failed_courses_count",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "identified_risks",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "is_current_course_third_attempt",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "passed_courses_count",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "sap_status",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "stu_id",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "support_received",
                table: "student_profiles");
        }
    }
}
