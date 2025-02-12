using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
    {
        public void Configure(EntityTypeBuilder<StudentEntity> builder)
        {
            builder.ToTable("students");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<StudentEntity> builder)
        {
            builder.HasKey(student => student.Id);
            builder.Property(student => student.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(student => student.Email)
                .HasColumnName("email")
                .IsRequired();
            builder.Property(student => student.Uuid)
                .HasColumnName("uuid")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<StudentEntity> builder)
        {
            builder.HasOne(student => student.StudentDetail)
                .WithOne(studentDetail => studentDetail.Student)
                .HasForeignKey<StudentDetailEntity>(studentDetail => studentDetail.StudentId);

            builder.HasMany(student => student.PollInstances)
                .WithOne(pollInstance => pollInstance.Student)
                .HasForeignKey(pollInstance => pollInstance.StudentId);

            builder.HasMany(student => student.Cohorts)
                .WithMany(cohort => cohort.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "student_cohort",
                    join => join
                        .HasOne<CohortEntity>()
                        .WithMany()
                        .HasForeignKey("cohort_id"),
                    join => join
                        .HasOne<StudentEntity>()
                        .WithMany()
                        .HasForeignKey("student_id"),
                    join =>
                    {
                        join.HasKey("student_id", "cohort_id");
                    }
                );
        }
    }
}