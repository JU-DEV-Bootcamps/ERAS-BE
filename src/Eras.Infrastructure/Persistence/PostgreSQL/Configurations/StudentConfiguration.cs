using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("students");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<Student> builder)
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

        private static void ConfigureRelationShips(EntityTypeBuilder<Student> builder)
        {
            builder.HasOne(student => student.StudentDetail)
                .WithOne(studentDetail => studentDetail.Student)
                .HasForeignKey<Student>(e => e.StudentDetail);

            builder.HasMany(student => student.PollInstances)
                .WithOne(pollInstance => pollInstance.Student)
                .HasForeignKey(pollInstance => pollInstance.Student);

            builder.HasMany(student => student.Cohorts)
                .WithMany(cohort => cohort.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "student_cohort",
                    join => join
                        .HasOne<Cohort>()
                        .WithMany()
                        .HasForeignKey("cohort_id"),
                    join => join
                        .HasOne<Student>()
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