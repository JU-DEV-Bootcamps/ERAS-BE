using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class FixJUEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "student_id",
                table: "ju_remissions");

            migrationBuilder.AddColumn<int[]>(
                name: "StudentIds",
                table: "ju_remissions",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentIds",
                table: "ju_remissions");

            migrationBuilder.AddColumn<int>(
                name: "student_id",
                table: "ju_remissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
