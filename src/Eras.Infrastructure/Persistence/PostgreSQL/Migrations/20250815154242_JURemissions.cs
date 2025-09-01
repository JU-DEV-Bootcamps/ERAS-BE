using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class JURemissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ju_interventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    diagnostic = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    objective = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ju_interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ju_interventions_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ju_services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    service_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ju_services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ju_remissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    submitter_uuid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ju_service_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_professional_uuid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    JUInterventionEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ju_remissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ju_remissions_ju_interventions_JUInterventionEntityId",
                        column: x => x.JUInterventionEntityId,
                        principalTable: "ju_interventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ju_remissions_ju_services_ju_service_id",
                        column: x => x.ju_service_id,
                        principalTable: "ju_services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ju_remissions_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ju_interventions_student_id",
                table: "ju_interventions",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_ju_remissions_ju_service_id",
                table: "ju_remissions",
                column: "ju_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_ju_remissions_JUInterventionEntityId",
                table: "ju_remissions",
                column: "JUInterventionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ju_remissions_student_id",
                table: "ju_remissions",
                column: "student_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ju_remissions");

            migrationBuilder.DropTable(
                name: "ju_interventions");

            migrationBuilder.DropTable(
                name: "ju_services");
        }
    }
}
