using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class add_student_complete_flag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.AddColumn<bool>(
                name: "is_imported",
                table: "students",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropColumn(
                name: "is_imported",
                table: "students");
        }
    }
}
