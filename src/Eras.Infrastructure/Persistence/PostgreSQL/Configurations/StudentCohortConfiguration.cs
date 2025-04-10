using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentCohortConfiguration : IEntityTypeConfiguration<StudentCohortJoin>
    {
        public void Configure(EntityTypeBuilder<StudentCohortJoin> builder)
        {
            builder.ToTable("student_cohort");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<StudentCohortJoin> builder)
        {
            builder.HasOne(studentCohort => studentCohort.Student)
                .WithMany(student => student.StudentCohorts)
                .HasForeignKey(studentCohort => studentCohort.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(studentCohort => studentCohort.Cohort)
                .WithMany(cohort => cohort.StudentCohorts)
                .HasForeignKey(studentCohort => studentCohort.CohortId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureColumns(EntityTypeBuilder<StudentCohortJoin> builder)
        {

            builder.HasKey(studentCohort => studentCohort.Id);
            builder.Property(studentCohort => studentCohort.StudentId)
                .HasColumnName("student_id")
                .IsRequired();
            builder.Property(studentCohort => studentCohort.CohortId)
                .HasColumnName("cohort_id")
                .IsRequired();
        }
    }
}
