using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class change_answer_atributtes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_ComponentVariables_ComponentVariableId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Polls_PollId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Students_StudentId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_ComponentVariableId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_PollId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_StudentId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "PollId",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Answers",
                newName: "Position");

            migrationBuilder.AddColumn<string>(
                name: "AnswerText",
                table: "Answers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "Answers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Answers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerText",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Answers",
                newName: "StudentId");

            migrationBuilder.AddColumn<int>(
                name: "PollId",
                table: "Answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_ComponentVariableId",
                table: "Answers",
                column: "ComponentVariableId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_PollId",
                table: "Answers",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_StudentId",
                table: "Answers",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_ComponentVariables_ComponentVariableId",
                table: "Answers",
                column: "ComponentVariableId",
                principalTable: "ComponentVariables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Polls_PollId",
                table: "Answers",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Students_StudentId",
                table: "Answers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
