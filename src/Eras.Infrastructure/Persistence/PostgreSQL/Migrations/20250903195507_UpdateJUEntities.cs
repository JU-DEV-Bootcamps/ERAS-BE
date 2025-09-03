using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJUEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_interventions_students_student_id",
                table: "ju_interventions");

            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_professionals_AssignedProfessionalId",
                table: "ju_remissions");

            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_services_ju_service_id",
                table: "ju_remissions");

            migrationBuilder.DropIndex(
                name: "IX_ju_remissions_ju_service_id",
                table: "ju_remissions");

            migrationBuilder.DropIndex(
                name: "IX_ju_interventions_student_id",
                table: "ju_interventions");

            migrationBuilder.DropColumn(
                name: "assigned_professional_uuid",
                table: "ju_remissions");

            migrationBuilder.RenameColumn(
                name: "AssignedProfessionalId",
                table: "ju_remissions",
                newName: "assigned_professional_id");

            migrationBuilder.RenameIndex(
                name: "IX_ju_remissions_AssignedProfessionalId",
                table: "ju_remissions",
                newName: "IX_ju_remissions_assigned_professional_id");

            migrationBuilder.AlterColumn<int>(
                name: "position",
                table: "variables",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_professionals_assigned_professional_id",
                table: "ju_remissions",
                column: "assigned_professional_id",
                principalTable: "ju_professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_professionals_assigned_professional_id",
                table: "ju_remissions");

            migrationBuilder.RenameColumn(
                name: "assigned_professional_id",
                table: "ju_remissions",
                newName: "AssignedProfessionalId");

            migrationBuilder.RenameIndex(
                name: "IX_ju_remissions_assigned_professional_id",
                table: "ju_remissions",
                newName: "IX_ju_remissions_AssignedProfessionalId");

            migrationBuilder.AlterColumn<int>(
                name: "position",
                table: "variables",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "assigned_professional_uuid",
                table: "ju_remissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ju_remissions_ju_service_id",
                table: "ju_remissions",
                column: "ju_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_ju_interventions_student_id",
                table: "ju_interventions",
                column: "student_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ju_interventions_students_student_id",
                table: "ju_interventions",
                column: "student_id",
                principalTable: "students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_professionals_AssignedProfessionalId",
                table: "ju_remissions",
                column: "AssignedProfessionalId",
                principalTable: "ju_professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_services_ju_service_id",
                table: "ju_remissions",
                column: "ju_service_id",
                principalTable: "ju_services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
