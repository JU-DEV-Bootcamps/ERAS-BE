using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CLConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "serviceProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceProviderName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ServiceProviderLogo = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serviceProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "configurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BaseURL = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EncryptedKey = table.Column<string>(type: "text", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_configurations_serviceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "serviceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userPolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PollId = table.Column<int>(type: "integer", nullable: false),
                    ConfigurationId = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userPolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userPolls_configurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "configurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userPolls_polls_PollId",
                        column: x => x.PollId,
                        principalTable: "polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_configurations_ServiceProviderId",
                table: "configurations",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_userPolls_ConfigurationId",
                table: "userPolls",
                column: "ConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_userPolls_PollId",
                table: "userPolls",
                column: "PollId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userPolls");

            migrationBuilder.DropTable(
                name: "configurations");

            migrationBuilder.DropTable(
                name: "serviceProviders");
        }
    }
}
