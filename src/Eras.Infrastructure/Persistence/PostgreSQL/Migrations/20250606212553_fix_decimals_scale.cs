using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class fix_decimals_scale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "standard_score_diff",
                table: "student_details",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "pure_score_diff",
                table: "student_details",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "courses_under_avg",
                table: "student_details",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "avg_score",
                table: "student_details",
                type: "numeric(14,4)",
                precision: 14,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "risk_level",
                table: "answers",
                type: "numeric(7,4)",
                precision: 7,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,4)",
                oldPrecision: 3,
                oldScale: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "standard_score_diff",
                table: "student_details",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,4)",
                oldPrecision: 14,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "pure_score_diff",
                table: "student_details",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,4)",
                oldPrecision: 14,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "courses_under_avg",
                table: "student_details",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,4)",
                oldPrecision: 14,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "avg_score",
                table: "student_details",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(14,4)",
                oldPrecision: 14,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "risk_level",
                table: "answers",
                type: "numeric(3,4)",
                precision: 3,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,4)",
                oldPrecision: 7,
                oldScale: 4);
        }
    }
}
