using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <summary>
    /// Data-only migration: an evaluation can hold several polls, but older imports only linked the
    /// first one in evaluation_poll. vErasEvaluationDetails now joins each poll instance to its own
    /// poll (poll_instances.uuid), so every poll that has instances must be linked to its evaluation.
    /// Insert-only and idempotent; nothing is deleted.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261009150000_BackfillEvaluationPolls")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class BackfillEvaluationPolls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO evaluation_poll (evaluation_id, poll_id)
                SELECT DISTINCT pi.""EvaluationId"", p.""Id""
                FROM poll_instances pi
                JOIN polls p ON p.uuid = pi.uuid
                WHERE pi.""EvaluationId"" IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1 FROM evaluation_poll ep
                      WHERE ep.evaluation_id = pi.""EvaluationId"" AND ep.poll_id = p.""Id"");");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The linked rows are valid data; they are intentionally kept.
        }
    }
}
