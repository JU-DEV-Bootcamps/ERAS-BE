using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class evaluationsentityupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "configuration_id",
                table: "evaluation",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_evaluation_configuration_id",
                table: "evaluation",
                column: "configuration_id");

            migrationBuilder.AddForeignKey(
                name: "FK_evaluation_configurations_configuration_id",
                table: "evaluation",
                column: "configuration_id",
                principalTable: "configurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_evaluation_configurations_configuration_id",
                table: "evaluation");

            migrationBuilder.DropIndex(
                name: "IX_evaluation_configuration_id",
                table: "evaluation");

            migrationBuilder.DropColumn(
                name: "configuration_id",
                table: "evaluation");
        }
    }
}
