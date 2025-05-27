using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class update_evaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.AddColumn<string>(
                name: "country",
                table: "evaluation",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder MigrationBuilder)
        {
            MigrationBuilder.DropColumn(
                name: "country",
                table: "evaluation");
        }
    }
}
