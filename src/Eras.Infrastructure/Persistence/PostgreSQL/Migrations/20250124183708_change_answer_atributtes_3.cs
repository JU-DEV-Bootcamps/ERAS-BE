using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class change_answer_atributtes_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComponentVariables_ComponentVariables_ParentId",
                table: "ComponentVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_Rules_ComponentVariables_ComponentVariableId",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Rules_ComponentVariableId",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_ComponentVariables_ParentId",
                table: "ComponentVariables");

            migrationBuilder.DropColumn(
                name: "ComponentVariableId",
                table: "Rules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComponentVariableId",
                table: "Rules",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ComponentVariableId",
                table: "Rules",
                column: "ComponentVariableId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentVariables_ParentId",
                table: "ComponentVariables",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComponentVariables_ComponentVariables_ParentId",
                table: "ComponentVariables",
                column: "ParentId",
                principalTable: "ComponentVariables",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_ComponentVariables_ComponentVariableId",
                table: "Rules",
                column: "ComponentVariableId",
                principalTable: "ComponentVariables",
                principalColumn: "Id");
        }
    }
}
