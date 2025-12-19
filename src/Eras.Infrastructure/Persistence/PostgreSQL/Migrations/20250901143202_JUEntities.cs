using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class JUEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_students_student_id",
                table: "ju_remissions");

            migrationBuilder.DropIndex(
                name: "IX_ju_remissions_student_id",
                table: "ju_remissions");

            migrationBuilder.AddColumn<int>(
                name: "AssignedProfessionalId",
                table: "ju_remissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ju_professionals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    uuid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ju_professionals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "student_remissions",
                columns: table => new
                {
                    RemissionsId = table.Column<int>(type: "integer", nullable: false),
                    StudentsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_remissions", x => new { x.RemissionsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_student_remissions_ju_remissions_RemissionsId",
                        column: x => x.RemissionsId,
                        principalTable: "ju_remissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_student_remissions_students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ju_remissions_AssignedProfessionalId",
                table: "ju_remissions",
                column: "AssignedProfessionalId");

            migrationBuilder.CreateIndex(
                name: "IX_student_remissions_StudentsId",
                table: "student_remissions",
                column: "StudentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_ju_professionals_AssignedProfessionalId",
                table: "ju_remissions",
                column: "AssignedProfessionalId",
                principalTable: "ju_professionals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ju_remissions_ju_professionals_AssignedProfessionalId",
                table: "ju_remissions");

            migrationBuilder.DropTable(
                name: "ju_professionals");

            migrationBuilder.DropTable(
                name: "student_remissions");

            migrationBuilder.DropIndex(
                name: "IX_ju_remissions_AssignedProfessionalId",
                table: "ju_remissions");

            migrationBuilder.DropColumn(
                name: "AssignedProfessionalId",
                table: "ju_remissions");

            migrationBuilder.CreateIndex(
                name: "IX_ju_remissions_student_id",
                table: "ju_remissions",
                column: "student_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ju_remissions_students_student_id",
                table: "ju_remissions",
                column: "student_id",
                principalTable: "students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
