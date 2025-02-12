using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class CohortConfiguration : IEntityTypeConfiguration<CohortEntity>
    {
        public void Configure(EntityTypeBuilder<CohortEntity> builder)
        {
            builder.ToTable("cohorts");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<CohortEntity> builder)
        {
            builder.HasKey(cohort => cohort.Id);
            builder.Property(cohort => cohort.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(cohort => cohort.CourseCode)
                .HasColumnName("course_name")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<CohortEntity> builder)
        {
            builder.HasMany(cohort => cohort.Students)
                .WithMany(student => student.Cohorts)
                .UsingEntity<Dictionary<string, object>>(
                    "student_cohort",
                    join => join
                        .HasOne<StudentEntity>()
                        .WithMany()
                        .HasForeignKey("student_id"),
                    join => join
                        .HasOne<CohortEntity>()
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