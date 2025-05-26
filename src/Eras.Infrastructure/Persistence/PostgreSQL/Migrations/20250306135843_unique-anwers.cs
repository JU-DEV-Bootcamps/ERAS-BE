using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class UniqueAnwers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropIndex(
                name: "IX_answers_poll_instance_id",
                table: "answers");

            MigrationBuilder.AddUniqueConstraint(
                name: "Unique_PollInstanceId_PollVariableId_AnswerText",
                table: "answers",
                columns: new[] { "poll_instance_id", "poll_variable_id", "answer_text" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropUniqueConstraint(
                name: "Unique_PollInstanceId_PollVariableId_AnswerText",
                table: "answers");

            MigrationBuilder.CreateIndex(
                name: "IX_answers_poll_instance_id",
                table: "answers",
                column: "poll_instance_id");
        }
    }
}
