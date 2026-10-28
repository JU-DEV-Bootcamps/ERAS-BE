using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AssessmentAssignedProfessionalsList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "assigned_professionals",
                table: "remissions",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            // Carry the single professional over as a one-item list.
            migrationBuilder.Sql(@"
                UPDATE remissions
                SET assigned_professionals = ARRAY[assigned_professional]
                WHERE assigned_professional IS NOT NULL AND assigned_professional <> '';");

            migrationBuilder.DropColumn(
                name: "assigned_professional",
                table: "remissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "assigned_professional",
                table: "remissions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            // Only the first professional of each list survives a rollback.
            migrationBuilder.Sql(@"
                UPDATE remissions
                SET assigned_professional = assigned_professionals[1]
                WHERE cardinality(assigned_professionals) > 0;");

            migrationBuilder.DropColumn(
                name: "assigned_professionals",
                table: "remissions");
        }
    }
}
