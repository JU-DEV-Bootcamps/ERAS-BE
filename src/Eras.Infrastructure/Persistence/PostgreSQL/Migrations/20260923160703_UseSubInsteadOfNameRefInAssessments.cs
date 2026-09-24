using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eras.Infrastructure.Persistence.PostgreSQL.Migrations
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class UseSubInsteadOfNameRefInAssessments : Migration
    {
        /// <summary>
        /// Looks for a user in eras_users table using the created_by and
        /// assigned_professional values (full names) and replaces them
        /// with the user sub. If there is no match, ignores the row.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE remissions r
                SET created_by = eu.sub
                FROM eras_users eu
                WHERE LOWER(TRIM(r.created_by)) = LOWER(TRIM(eu.first_name || ' ' || eu.last_name))
                    AND r.created_by IS NOT NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE remissions r
                SET assigned_professional = eu.sub
                FROM eras_users eu
                WHERE LOWER(TRIM(r.assigned_professional)) = LOWER(TRIM(eu.first_name || ' ' || eu.last_name))
                    AND r.assigned_professional IS NOT NULL;
            ");
        }

        /// <summary>
        /// Looks for a user in eras_users table using the created_by and
        /// assigned_professional values (subs) and replaces them
        /// with the user full name. If there is no match, ignores the row.
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE remissions r
                SET created_by = eu.first_name || ' ' || eu.last_name
                FROM eras_users eu
                WHERE r.created_by = eu.sub
                    AND r.created_by IS NOT NULL;
            ");

            migrationBuilder.Sql(@"
                UPDATE remissions r
                SET assigned_professional = eu.first_name || ' ' || eu.last_name
                FROM eras_users eu
                WHERE r.assigned_professional = eu.sub
                    AND r.assigned_professional IS NOT NULL;
            ");
        }
    }
}
