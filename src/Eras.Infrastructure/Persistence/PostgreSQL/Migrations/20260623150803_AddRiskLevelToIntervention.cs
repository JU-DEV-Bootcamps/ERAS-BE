using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddRiskLevelToIntervention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "riskLevel",
                table: "Interventions",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_poll_instances_EvaluationId",
                table: "poll_instances",
                column: "EvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_poll_instances_evaluation_EvaluationId",
                table: "poll_instances",
                column: "EvaluationId",
                principalTable: "evaluation",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_poll_instances_evaluation_EvaluationId",
                table: "poll_instances");

            migrationBuilder.DropIndex(
                name: "IX_poll_instances_EvaluationId",
                table: "poll_instances");

            migrationBuilder.DropColumn(
                name: "riskLevel",
                table: "Interventions");
        }
    }
}
