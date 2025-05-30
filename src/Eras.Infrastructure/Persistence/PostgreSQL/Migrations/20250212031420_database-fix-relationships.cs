using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations;

/// <inheritdoc />
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class DatabaseFixRelationships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder MigrationBuilder)
    {
        MigrationBuilder.DropForeignKey(
            name: "FK_answers_poll_instances_PollInstanceId",
            table: "answers");

        MigrationBuilder.DropForeignKey(
            name: "FK_cohorts_variables_VariableId",
            table: "cohorts");

        MigrationBuilder.DropForeignKey(
            name: "FK_variables_components_ComponentId",
            table: "variables");

        MigrationBuilder.DropPrimaryKey(
            name: "PK_poll_variable",
            table: "poll_variable");

        MigrationBuilder.DropIndex(
            name: "IX_cohorts_VariableId",
            table: "cohorts");

        MigrationBuilder.DropColumn(
            name: "VariableId",
            table: "cohorts");

        MigrationBuilder.RenameColumn(
            name: "ComponentId",
            table: "variables",
            newName: "component_id");

        MigrationBuilder.RenameIndex(
            name: "IX_variables_ComponentId",
            table: "variables",
            newName: "IX_variables_component_id");

        MigrationBuilder.RenameColumn(
            name: "PollInstanceId",
            table: "answers",
            newName: "poll_instance_id");

        MigrationBuilder.RenameIndex(
            name: "IX_answers_PollInstanceId",
            table: "answers",
            newName: "IX_answers_poll_instance_id");

        MigrationBuilder.AddColumn<int>(
            name: "Id",
            table: "poll_variable",
            type: "integer",
            nullable: false,
            defaultValue: 0)
            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

        MigrationBuilder.AddColumn<int>(
            name: "poll_variable_id",
            table: "answers",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        MigrationBuilder.AddPrimaryKey(
            name: "PK_poll_variable",
            table: "poll_variable",
            column: "Id");

        MigrationBuilder.CreateIndex(
            name: "IX_poll_variable_poll_id",
            table: "poll_variable",
            column: "poll_id");

        MigrationBuilder.CreateIndex(
            name: "IX_answers_poll_variable_id",
            table: "answers",
            column: "poll_variable_id");

        MigrationBuilder.AddForeignKey(
            name: "FK_answers_poll_instances_poll_instance_id",
            table: "answers",
            column: "poll_instance_id",
            principalTable: "poll_instances",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        MigrationBuilder.AddForeignKey(
            name: "FK_answers_poll_variable_poll_variable_id",
            table: "answers",
            column: "poll_variable_id",
            principalTable: "poll_variable",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        MigrationBuilder.AddForeignKey(
            name: "FK_variables_components_component_id",
            table: "variables",
            column: "component_id",
            principalTable: "components",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder MigrationBuilder)
    {
        MigrationBuilder.DropForeignKey(
            name: "FK_answers_poll_instances_poll_instance_id",
            table: "answers");

        MigrationBuilder.DropForeignKey(
            name: "FK_answers_poll_variable_poll_variable_id",
            table: "answers");

        MigrationBuilder.DropForeignKey(
            name: "FK_variables_components_component_id",
            table: "variables");

        MigrationBuilder.DropPrimaryKey(
            name: "PK_poll_variable",
            table: "poll_variable");

        MigrationBuilder.DropIndex(
            name: "IX_poll_variable_poll_id",
            table: "poll_variable");

        MigrationBuilder.DropIndex(
            name: "IX_answers_poll_variable_id",
            table: "answers");

        MigrationBuilder.DropColumn(
            name: "Id",
            table: "poll_variable");

        MigrationBuilder.DropColumn(
            name: "poll_variable_id",
            table: "answers");

        MigrationBuilder.RenameColumn(
            name: "component_id",
            table: "variables",
            newName: "ComponentId");

        MigrationBuilder.RenameIndex(
            name: "IX_variables_component_id",
            table: "variables",
            newName: "IX_variables_ComponentId");

        MigrationBuilder.RenameColumn(
            name: "poll_instance_id",
            table: "answers",
            newName: "PollInstanceId");

        MigrationBuilder.RenameIndex(
            name: "IX_answers_poll_instance_id",
            table: "answers",
            newName: "IX_answers_PollInstanceId");

        MigrationBuilder.AddColumn<int>(
            name: "VariableId",
            table: "cohorts",
            type: "integer",
            nullable: true);

        MigrationBuilder.AddPrimaryKey(
            name: "PK_poll_variable",
            table: "poll_variable",
            columns: ["poll_id", "variable_id"]);

        MigrationBuilder.CreateIndex(
            name: "IX_cohorts_VariableId",
            table: "cohorts",
            column: "VariableId");

        MigrationBuilder.AddForeignKey(
            name: "FK_answers_poll_instances_PollInstanceId",
            table: "answers",
            column: "PollInstanceId",
            principalTable: "poll_instances",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        MigrationBuilder.AddForeignKey(
            name: "FK_cohorts_variables_VariableId",
            table: "cohorts",
            column: "VariableId",
            principalTable: "variables",
            principalColumn: "Id");

        MigrationBuilder.AddForeignKey(
            name: "FK_variables_components_ComponentId",
            table: "variables",
            column: "ComponentId",
            principalTable: "components",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
