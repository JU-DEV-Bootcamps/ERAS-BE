using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class add_relation_pollvariable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ComponentVariables_PollId",
                table: "ComponentVariables",
                column: "PollId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentVariables_Polls_PollId",
                table: "ComponentVariables",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentVariables_Polls_PollId",
                table: "ComponentVariables");

            migrationBuilder.DropIndex(
                name: "IX_ComponentVariables_PollId",
                table: "ComponentVariables");
        }
    }
}
