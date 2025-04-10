using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentDetailConfiguration : IEntityTypeConfiguration<StudentDetailEntity>
    {
        public void Configure(EntityTypeBuilder<StudentDetailEntity> builder)
        {
            builder.ToTable("student_details");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<StudentDetailEntity> builder)
        {
            builder.HasKey(detail => detail.Id);
            builder.Property(detail => detail.EnrolledCourses)
                .HasColumnName("enrolled_courses")
                .IsRequired();
            builder.Property(detail => detail.GradedCourses)
                .HasColumnName("graded_courses")
                .IsRequired();
            builder.Property(detail => detail.TimeDeliveryRate)
                .HasColumnName("time_delivery_rate")
                .IsRequired();
            builder.Property(detail => detail.AvgScore)
                .HasColumnName("avg_score")
                .IsRequired();
            builder.Property(detail => detail.CoursesUnderAvg)
                .HasColumnName("courses_under_avg")
                .IsRequired();
            builder.Property(detail => detail.PureScoreDiff)
                .HasColumnName("pure_score_diff")
                .IsRequired();
            builder.Property(detail => detail.StandardScoreDiff)
                .HasColumnName("standard_score_diff")
                .IsRequired();
            builder.Property(detail => detail.LastAccessDays)
                .HasColumnName("last_access_days")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<StudentDetailEntity> builder)
        {
            builder.HasOne(detail => detail.Student)
                .WithOne(student => student.StudentDetail)
                .HasForeignKey<StudentDetailEntity>(detail => detail.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
