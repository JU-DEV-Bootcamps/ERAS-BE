using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class RenameStatusesInterventionsAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "Interventions",
                defaultValue: "Remitted",
                oldDefaultValue: "Created");
            migrationBuilder.Sql(@"
                UPDATE remissions
                SET status = 'Remitted'
                WHERE status = 'Created';

                UPDATE remissions
                SET status = 'Finalized'
                WHERE status = 'Resolved';

                UPDATE remissions
                SET status = 'InProgress'
                WHERE status IN ('OnHold', 'Rejected');
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Interventions""
                SET status = 'Remitted'
                WHERE status = 'Created';

                UPDATE ""Interventions""
                SET status = 'Finalized'
                WHERE status = 'Resolved';

                UPDATE ""Interventions""
                SET status = 'InProgress'
                WHERE status IN ('OnHold', 'Rejected');
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "Interventions",
                defaultValue: "Created",
                oldDefaultValue: "Remitted");
            migrationBuilder.Sql(@"
                UPDATE remissions
                SET status = 'Created'
                WHERE status = 'Remitted';

                UPDATE remissions
                SET status = 'Resolved'
                WHERE status = 'Finalized';
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Interventions""
                SET status = 'Created'
                WHERE status = 'Remitted';

                UPDATE ""Interventions""
                SET status = 'Resolved'
                WHERE status = 'Finalized';
            ");
        }
    }
}
