using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddSourcePollInstanceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourcePollInstanceId",
                table: "poll_instances",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                WITH ranked AS (
                    SELECT 
                        pi.""Id"",
                        pi.""StudentId"",
                        pi.""FinishedAt"",
                        FIRST_VALUE(pi.""Id"") OVER (
                            PARTITION BY pi.""StudentId"" 
                            ORDER BY pi.""FinishedAt"" ASC
                        ) AS original_id
                    FROM poll_instances pi
                )
                UPDATE poll_instances
                SET ""SourcePollInstanceId"" = ranked.original_id
                FROM ranked
                WHERE poll_instances.""Id"" = ranked.""Id""
                AND ranked.""Id"" != ranked.original_id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourcePollInstanceId",
                table: "poll_instances");
        }
    }
}
