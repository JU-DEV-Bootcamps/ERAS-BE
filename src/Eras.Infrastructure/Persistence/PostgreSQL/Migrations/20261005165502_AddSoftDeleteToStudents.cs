using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_student_profiles_id_passport_number",
                table: "student_profiles");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "students",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "student_profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ux_student_profiles_id_passport_number",
                table: "student_profiles",
                column: "id_passport_number",
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_student_profiles_id_passport_number",
                table: "student_profiles");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "students");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "student_profiles");

            migrationBuilder.CreateIndex(
                name: "ux_student_profiles_id_passport_number",
                table: "student_profiles",
                column: "id_passport_number",
                unique: true);
        }
    }
}
