using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class JUInterventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions");

            migrationBuilder.DropUniqueConstraint(
                name: "Unique_PollInstanceId_PollVariableId_AnswerText",
                table: "answers");

            migrationBuilder.CreateIndex(
                name: "IX_answers_poll_instance_id",
                table: "answers",
                column: "poll_instance_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions",
                column: "JUInterventionEntityId",
                principalTable: "ju_interventions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions");

            migrationBuilder.DropIndex(
                name: "IX_answers_poll_instance_id",
                table: "answers");

            migrationBuilder.AddUniqueConstraint(
                name: "Unique_PollInstanceId_PollVariableId_AnswerText",
                table: "answers",
                columns: new[] { "poll_instance_id", "poll_variable_id", "answer_text" });

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions",
                column: "JUInterventionEntityId",
                principalTable: "ju_interventions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
