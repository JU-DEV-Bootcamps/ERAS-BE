using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.AddColumn<int>(
                name: "position",
                table: "variables",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            MigrationBuilder.Sql(@"
                UPDATE variables 
                SET position = subquery.new_position 
                FROM (
                    SELECT 
                        v.""Id"",
                        ROW_NUMBER() OVER (
                            PARTITION BY pv.poll_id 
                            ORDER BY pv.created_at ASC, pv.""Id"" ASC
                        ) as new_position
                    FROM variables v
                    JOIN poll_variable pv ON v.""Id"" = pv.variable_id
                ) AS subquery 
                WHERE variables.""Id"" = subquery.""Id"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropColumn(
                name: "position",
                table: "variables");
        }
    }
}
