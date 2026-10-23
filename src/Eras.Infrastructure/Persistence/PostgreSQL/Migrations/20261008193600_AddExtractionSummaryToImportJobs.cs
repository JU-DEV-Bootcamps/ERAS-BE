using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddExtractionSummaryToImportJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "returned_count",
                table: "import_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "skipped_invalid_answers",
                table: "import_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "skipped_outside_date_range",
                table: "import_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "skipped_request_failed",
                table: "import_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "skipped_without_score",
                table: "import_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "returned_count",
                table: "import_jobs");

            migrationBuilder.DropColumn(
                name: "skipped_invalid_answers",
                table: "import_jobs");

            migrationBuilder.DropColumn(
                name: "skipped_outside_date_range",
                table: "import_jobs");

            migrationBuilder.DropColumn(
                name: "skipped_request_failed",
                table: "import_jobs");

            migrationBuilder.DropColumn(
                name: "skipped_without_score",
                table: "import_jobs");
        }
    }
}
