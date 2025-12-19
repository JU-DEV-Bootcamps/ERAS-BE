using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "serviceProviders",
                columns: new[] { "Id", "ServiceProviderLogo", "ServiceProviderName", "created_at", "created_by", "updated_at", "modified_by" },
                values: new object[] { 1, "https://i.imgur.com/cDQU1M7.png", "Cosmic Latte", new DateTime(2025, 12, 1, 23, 37, 1, 816, DateTimeKind.Utc).AddTicks(9678), "System", null, "System" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "serviceProviders",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
