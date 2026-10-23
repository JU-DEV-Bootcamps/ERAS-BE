using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class SetV2FeatureFlagEnabledByDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO feature_flag (name, description, is_enabled, created_by, created_at)
VALUES ('v2', 'Enables the V2 interface and the enhanced Cosmic Latte import', TRUE, 'System', NOW())
ON CONFLICT (name) DO UPDATE
    SET is_enabled = TRUE, modified_by = 'System', updated_at = NOW();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE feature_flag
SET is_enabled = FALSE, modified_by = 'System', updated_at = NOW()
WHERE name = 'v2';");
        }
    }
}
