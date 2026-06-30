using System;

using Eras.Infrastructure.Persistence.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260630190000_AddExtractionFields")]
    public partial class AddExtractionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "extracted_count", table: "import_jobs", type: "integer", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<string>(
                name: "evaluation_set_name", table: "import_jobs", type: "character varying(200)", maxLength: 200, nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "configuration_id", table: "import_jobs", type: "integer", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<string>(
                name: "start_date", table: "import_jobs", type: "character varying(40)", maxLength: 40, nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "end_date", table: "import_jobs", type: "character varying(40)", maxLength: 40, nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_already_imported", table: "import_job_items", type: "boolean", nullable: false, defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "extracted_count", table: "import_jobs");
            migrationBuilder.DropColumn(name: "evaluation_set_name", table: "import_jobs");
            migrationBuilder.DropColumn(name: "configuration_id", table: "import_jobs");
            migrationBuilder.DropColumn(name: "start_date", table: "import_jobs");
            migrationBuilder.DropColumn(name: "end_date", table: "import_jobs");
            migrationBuilder.DropColumn(name: "is_already_imported", table: "import_job_items");
        }
    }
}
