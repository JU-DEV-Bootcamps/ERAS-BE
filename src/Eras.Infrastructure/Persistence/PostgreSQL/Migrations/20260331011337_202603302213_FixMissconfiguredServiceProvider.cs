using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class _202603302213_FixMissconfiguredServiceProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "serviceProviders",
                keyColumn: "Id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 3, 31, 1, 6, 56, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "serviceProviders",
                keyColumn: "Id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2026, 3, 31, 1, 6, 56, 98, DateTimeKind.Utc).AddTicks(7626));
        }
    }
}
