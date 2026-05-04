using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    public partial class fixIdCoumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interventions_remissions_remission_id",
                table: "Interventions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_remissions",
                table: "remissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Interventions",
                table: "Interventions");

            migrationBuilder.DropColumn("remission_id", "Interventions");
            migrationBuilder.DropColumn("Id", "Interventions");
            migrationBuilder.DropColumn("id", "remissions");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "remissions",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Interventions",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "remission_id",
                table: "Interventions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_remissions",
                table: "remissions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interventions",
                table: "Interventions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interventions_remissions_remission_id",
                table: "Interventions",
                column: "remission_id",
                principalTable: "remissions",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}