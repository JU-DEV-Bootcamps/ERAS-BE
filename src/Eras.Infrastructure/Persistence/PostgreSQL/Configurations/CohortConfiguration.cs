using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class CohortConfiguration : IEntityTypeConfiguration<Cohort>
    {
        public void Configure(EntityTypeBuilder<Cohort> builder)
        {
            builder.ToTable("cohorts");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<Cohort> builder)
        {
            builder.HasKey(cohort => cohort.Id);
            builder.Property(cohort => cohort.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(cohort => cohort.CourseCode)
                .HasColumnName("course_name")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<Cohort> builder)
        {
            builder.HasMany(cohort => cohort.Students)
                .WithMany(student => student.Cohorts)
                .UsingEntity<Dictionary<string, object>>(
                    "student_cohort",
                    join => join
                        .HasOne<Student>()
                        .WithMany()
                        .HasForeignKey("student_id"),
                    join => join
                        .HasOne<Cohort>()
                        .WithMany()
                        .HasForeignKey("cohort_id"),
                    join =>
                    {
                        join.HasKey("cohort_id", "student_id");
                    }
                );
        }
    }
}