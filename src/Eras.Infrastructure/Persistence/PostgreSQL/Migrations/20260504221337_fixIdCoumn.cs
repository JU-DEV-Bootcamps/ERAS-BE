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

            migrationBuilder.AddColumn<int>(
                name: "new_id",
                table: "remissions",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "new_remission_id",
                table: "Interventions",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE ""Interventions"" i
                SET new_remission_id = r.""new_id""
                FROM ""remissions"" r
                WHERE i.""remission_id""::text = r.""id""::text
            ");

            migrationBuilder.DropColumn("remission_id", "Interventions");
            migrationBuilder.DropColumn("Id", "Interventions");
            migrationBuilder.DropColumn("id", "remissions");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Interventions",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.RenameColumn(
                name: "new_id",
                table: "remissions",
                newName: "id"
            );

            migrationBuilder.RenameColumn(
                name: "new_remission_id",
                table: "Interventions",
                newName: "remission_id"
            );

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
            migrationBuilder.DropForeignKey(
                name: "FK_Interventions_remissions_remission_id",
                table: "Interventions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_remissions",
                table: "remissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Interventions",
                table: "Interventions");

            migrationBuilder.AddColumn<Guid>(
                name: "new_id",
                table: "remissions",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.AddColumn<Guid>(
                name: "new_remission_id",
                table: "Interventions",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.Sql(@"
                UPDATE ""remissions""
                SET ""new_id"" = gen_random_uuid() WHERE ""new_id"" IS NULL
            ");

            migrationBuilder.Sql(@"UPDATE ""Interventions"" i
                SET new_remission_id = r.""new_id""
                FROM ""remissions"" r
                WHERE i.""remission_id"" = r.""id""
            ");

            migrationBuilder.DropColumn("remission_id", "Interventions");
            migrationBuilder.DropColumn("Id", "Interventions");
            migrationBuilder.DropColumn("id", "remissions");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Interventions",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.Sql(@"
                UPDATE ""Interventions""
                SET ""Id"" = gen_random_uuid() WHERE ""Id"" IS NULL
            ");

            migrationBuilder.RenameColumn(
                name: "new_id",
                table: "remissions",
                newName: "id"
            );

            migrationBuilder.RenameColumn(
                name: "new_remission_id",
                table: "Interventions",
                newName: "remission_id"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "remissions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Interventions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
            
            migrationBuilder.AlterColumn<Guid>(
                name: "remission_id",
                table: "Interventions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Interventions",
                table: "Interventions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_remissions",
                table: "remissions",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interventions_remissions_remission_id",
                table: "Interventions",
                column: "remission_id",
                principalTable: "remissions",
                principalColumn: "id");
        }
    }
}