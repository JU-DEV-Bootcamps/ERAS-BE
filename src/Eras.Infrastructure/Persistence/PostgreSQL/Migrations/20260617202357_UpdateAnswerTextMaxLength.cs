using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnswerTextMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "answer_text",
                table: "answers",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vErasCalculationByPoll;");
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS vErasEvaluationDetails;");

            migrationBuilder.AlterColumn<string>(
                name: "answer_text",
                table: "answers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);
            var viewsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Persistence", "PostgreSQL", "Views" ,"Ups");
            using StreamReader calcReader = new StreamReader(Path.Combine(viewsPath,"vErasCalculationByPoll.sql"));
            var calcViewScript = calcReader.ReadToEnd();
            migrationBuilder.Sql(calcViewScript);

            using StreamReader evalReader = new StreamReader(Path.Combine(viewsPath,"vErasEvaluationDetails.sql"));
            var evalReaderScript = evalReader.ReadToEnd();
            migrationBuilder.Sql(evalReaderScript);
        }
    }
}
