using Eras.Infrastructure.Persistence.PostgreSQL;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260630160000_AddAnswersHashToPollInstance")]
    public partial class AddAnswersHashToPollInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "answers_hash",
                table: "poll_instances",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_poll_instances_student_id_answers_hash",
                table: "poll_instances",
                columns: new[] { "StudentId", "answers_hash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_poll_instances_student_id_answers_hash",
                table: "poll_instances");

            migrationBuilder.DropColumn(
                name: "answers_hash",
                table: "poll_instances");
        }
    }
}
