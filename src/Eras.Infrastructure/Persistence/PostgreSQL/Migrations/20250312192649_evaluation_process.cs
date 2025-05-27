using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class evaluation_process : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.CreateTable(
                name: "evaluation",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = Table.Column<string>(type: "text", nullable: false),
                    status = Table.Column<string>(type: "text", nullable: false),
                    poll_name = Table.Column<string>(type: "text", nullable: false),
                    start_date = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = Table.Column<string>(type: "text", nullable: false),
                    modified_by = Table.Column<string>(type: "text", nullable: true),
                    created_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = Table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_evaluation", X => X.Id);
                });

            MigrationBuilder.CreateTable(
                name: "evaluation_poll",
                columns: Table => new
                {
                    Id = Table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    poll_id = Table.Column<int>(type: "integer", nullable: false),
                    evaluation_id = Table.Column<int>(type: "integer", nullable: false)
                },
                constraints: Table =>
                {
                    Table.PrimaryKey("PK_evaluation_poll", X => X.Id);
                    Table.ForeignKey(
                        name: "FK_evaluation_poll_evaluation_evaluation_id",
                        column: X => X.evaluation_id,
                        principalTable: "evaluation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    Table.ForeignKey(
                        name: "FK_evaluation_poll_polls_poll_id",
                        column: X => X.poll_id,
                        principalTable: "polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            MigrationBuilder.CreateIndex(
                name: "IX_evaluation_poll_evaluation_id",
                table: "evaluation_poll",
                column: "evaluation_id");

            MigrationBuilder.CreateIndex(
                name: "IX_evaluation_poll_poll_id",
                table: "evaluation_poll",
                column: "poll_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropTable(
                name: "evaluation_poll");

            MigrationBuilder.DropTable(
                name: "evaluation");
        }
    }
}
