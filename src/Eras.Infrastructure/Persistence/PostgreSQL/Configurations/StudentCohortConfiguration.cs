using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentCohortConfiguration : IEntityTypeConfiguration<StudentCohortJoin>
    {
        public void Configure(EntityTypeBuilder<StudentCohortJoin> Builder)
        {
            Builder.ToTable("student_cohort");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<StudentCohortJoin> Builder)
        {
            Builder.HasOne(StudentCohort => StudentCohort.Student)
                .WithMany(Student => Student.StudentCohorts)
                .HasForeignKey(StudentCohort => StudentCohort.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            Builder.HasOne(StudentCohort => StudentCohort.Cohort)
                .WithMany(Cohort => Cohort.StudentCohorts)
                .HasForeignKey(StudentCohort => StudentCohort.CohortId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureColumns(EntityTypeBuilder<StudentCohortJoin> Builder)
        {

            Builder.HasKey(StudentCohort => StudentCohort.Id);
            Builder.Property(StudentCohort => StudentCohort.StudentId)
                .HasColumnName("student_id")
                .IsRequired();
            Builder.Property(StudentCohort => StudentCohort.CohortId)
                .HasColumnName("cohort_id")
                .IsRequired();
        }
    }
}
