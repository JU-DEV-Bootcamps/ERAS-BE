using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class uniqueanwers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_answers_poll_instance_id",
                table: "answers");

            migrationBuilder.AddUniqueConstraint(
                name: "Unique_PollInstanceId_PollVariableId_AnswerText",
                table: "answers",
                columns: new[] { "poll_instance_id", "poll_variable_id", "answer_text" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "Unique_PollInstanceId_PollVariableId_AnswerText",
                table: "answers");

            migrationBuilder.CreateIndex(
                name: "IX_answers_poll_instance_id",
                table: "answers",
                column: "poll_instance_id");
        }
    }
}
