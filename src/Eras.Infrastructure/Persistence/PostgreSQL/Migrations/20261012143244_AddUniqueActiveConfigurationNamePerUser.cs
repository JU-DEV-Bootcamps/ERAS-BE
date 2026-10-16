using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueActiveConfigurationNamePerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE configurations AS c
SET ""ConfigurationName"" = c.""ConfigurationName"" || ' (' || c.""Id"" || ')'
WHERE c.""IsDeleted"" = FALSE
  AND EXISTS (
      SELECT 1
      FROM configurations AS d
      WHERE d.""UserId"" = c.""UserId""
        AND d.""ConfigurationName"" = c.""ConfigurationName""
        AND d.""IsDeleted"" = FALSE
        AND d.""Id"" > c.""Id"");");

            migrationBuilder.CreateIndex(
                name: "ux_configurations_user_id_configuration_name_active",
                table: "configurations",
                columns: new[] { "UserId", "ConfigurationName" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_configurations_user_id_configuration_name_active",
                table: "configurations");
        }
    }
}
