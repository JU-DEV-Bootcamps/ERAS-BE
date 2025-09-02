using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class JUInterventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions");


            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions",
                column: "JUInterventionEntityId",
                principalTable: "ju_interventions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions");

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                table: "ju_remissions",
                column: "JUInterventionEntityId",
                principalTable: "ju_interventions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
