using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class _202603302206_AssessmentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "remissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    service = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    assigned_professional = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    student_ids = table.Column<string>(type: "jsonb", nullable: false),
                    diagnosis = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    objective = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    comments = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    plan_sessions_per_week = table.Column<int>(type: "integer", nullable: true),
                    plan_schedule_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_remissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "student_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    support_and_referral_history = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    characterization_or_current_context = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActivityType = table.Column<string>(type: "text", nullable: true),
                    Professional = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    Attachments = table.Column<string[]>(type: "text[]", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    remission_id = table.Column<Guid>(type: "uuid", nullable: true),
                    area = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    participant_ids = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_remissions_remission_id",
                        column: x => x.remission_id,
                        principalTable: "remissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "serviceProviders",
                keyColumn: "Id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 3, 31, 1, 6, 56, 98, DateTimeKind.Utc).AddTicks(7626));

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_remission_id",
                table: "Interventions",
                column: "remission_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_profiles_student_code",
                table: "student_profiles",
                column: "student_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "student_profiles");

            migrationBuilder.DropTable(
                name: "remissions");

            migrationBuilder.UpdateData(
                table: "serviceProviders",
                keyColumn: "Id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2025, 12, 1, 23, 37, 1, 816, DateTimeKind.Utc).AddTicks(9678));
        }
    }
}
