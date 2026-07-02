using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleTableWithEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Email);
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Email", "Role", "CreatedAt" },
                values: new object[,]
                {
                    { "Lucia.Becerra@jalasoft.com", "ErasAdmin", DateTime.UtcNow },
                    { "Sebastian.Beltran@jalasoft.com", "ErasAdmin", DateTime.UtcNow },
                    { "Judith.Paco@jalasoft.com", "ErasAdmin", DateTime.UtcNow },
                    { "Jaroslav.Halas@jalasoft.com", "ErasAdmin", DateTime.UtcNow },
                    { "Marcel.Morales@jalasoft.com", "ErasAdmin", DateTime.UtcNow }
                
                });
            }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
