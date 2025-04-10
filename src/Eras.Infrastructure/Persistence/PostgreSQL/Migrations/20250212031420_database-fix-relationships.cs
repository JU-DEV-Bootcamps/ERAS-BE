using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class databasefixrelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_answers_poll_instances_PollInstanceId",
                table: "answers");

            migrationBuilder.DropForeignKey(
                name: "FK_cohorts_variables_VariableId",
                table: "cohorts");

            migrationBuilder.DropForeignKey(
                name: "FK_variables_components_ComponentId",
                table: "variables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_poll_variable",
                table: "poll_variable");

            migrationBuilder.DropIndex(
                name: "IX_cohorts_VariableId",
                table: "cohorts");

            migrationBuilder.DropColumn(
                name: "VariableId",
                table: "cohorts");

            migrationBuilder.RenameColumn(
                name: "ComponentId",
                table: "variables",
                newName: "component_id");

            migrationBuilder.RenameIndex(
                name: "IX_variables_ComponentId",
                table: "variables",
                newName: "IX_variables_component_id");

            migrationBuilder.RenameColumn(
                name: "PollInstanceId",
                table: "answers",
                newName: "poll_instance_id");

            migrationBuilder.RenameIndex(
                name: "IX_answers_PollInstanceId",
                table: "answers",
                newName: "IX_answers_poll_instance_id");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "poll_variable",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "poll_variable_id",
                table: "answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_poll_variable",
                table: "poll_variable",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_poll_variable_poll_id",
                table: "poll_variable",
                column: "poll_id");

            migrationBuilder.CreateIndex(
                name: "IX_answers_poll_variable_id",
                table: "answers",
                column: "poll_variable_id");

            migrationBuilder.AddForeignKey(
                name: "FK_answers_poll_instances_poll_instance_id",
                table: "answers",
                column: "poll_instance_id",
                principalTable: "poll_instances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_answers_poll_variable_poll_variable_id",
                table: "answers",
                column: "poll_variable_id",
                principalTable: "poll_variable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_variables_components_component_id",
                table: "variables",
                column: "component_id",
                principalTable: "components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_answers_poll_instances_poll_instance_id",
                table: "answers");

            migrationBuilder.DropForeignKey(
                name: "FK_answers_poll_variable_poll_variable_id",
                table: "answers");

            migrationBuilder.DropForeignKey(
                name: "FK_variables_components_component_id",
                table: "variables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_poll_variable",
                table: "poll_variable");

            migrationBuilder.DropIndex(
                name: "IX_poll_variable_poll_id",
                table: "poll_variable");

            migrationBuilder.DropIndex(
                name: "IX_answers_poll_variable_id",
                table: "answers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "poll_variable");

            migrationBuilder.DropColumn(
                name: "poll_variable_id",
                table: "answers");

            migrationBuilder.RenameColumn(
                name: "component_id",
                table: "variables",
                newName: "ComponentId");

            migrationBuilder.RenameIndex(
                name: "IX_variables_component_id",
                table: "variables",
                newName: "IX_variables_ComponentId");

            migrationBuilder.RenameColumn(
                name: "poll_instance_id",
                table: "answers",
                newName: "PollInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_answers_poll_instance_id",
                table: "answers",
                newName: "IX_answers_PollInstanceId");

            migrationBuilder.AddColumn<int>(
                name: "VariableId",
                table: "cohorts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_poll_variable",
                table: "poll_variable",
                columns: new[] { "poll_id", "variable_id" });

            migrationBuilder.CreateIndex(
                name: "IX_cohorts_VariableId",
                table: "cohorts",
                column: "VariableId");

            migrationBuilder.AddForeignKey(
                name: "FK_answers_poll_instances_PollInstanceId",
                table: "answers",
                column: "PollInstanceId",
                principalTable: "poll_instances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cohorts_variables_VariableId",
                table: "cohorts",
                column: "VariableId",
                principalTable: "variables",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_variables_components_ComponentId",
                table: "variables",
                column: "ComponentId",
                principalTable: "components",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
