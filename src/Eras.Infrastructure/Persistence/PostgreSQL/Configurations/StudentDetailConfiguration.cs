﻿using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentDetailConfiguration : IEntityTypeConfiguration<StudentDetailEntity>
    {
        public void Configure(EntityTypeBuilder<StudentDetailEntity> Builder)
        {
            Builder.ToTable("student_details");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<StudentDetailEntity> Builder)
        {
            Builder.HasKey(Detail => Detail.Id);
            Builder.Property(Detail => Detail.EnrolledCourses)
                .HasColumnName("enrolled_courses")
                .HasColumnType("smallint")
                .IsRequired();
            Builder.Property(Detail => Detail.GradedCourses)
                .HasColumnName("graded_courses")
                .HasColumnType("smallint")
                .IsRequired();
            Builder.Property(Detail => Detail.TimeDeliveryRate)
                .HasColumnName("time_delivery_rate")
                .HasColumnType("smallint")
                .IsRequired();
            Builder.Property(Detail => Detail.AvgScore)
                .HasColumnName("avg_score")
                .HasPrecision(14,4)
                .IsRequired();
            Builder.Property(Detail => Detail.CoursesUnderAvg)
                .HasColumnName("courses_under_avg")
                .HasPrecision(14, 4)
                .IsRequired();
            Builder.Property(Detail => Detail.PureScoreDiff)
                .HasColumnName("pure_score_diff")
                .HasPrecision(14, 4)
                .IsRequired();
            Builder.Property(Detail => Detail.StandardScoreDiff)
                .HasColumnName("standard_score_diff")
                .HasPrecision(14, 4)
                .IsRequired();
            Builder.Property(Detail => Detail.LastAccessDays)
                .HasColumnName("last_access_days")
                .HasColumnType("smallint")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<StudentDetailEntity> Builder)
        {
            Builder.HasOne(Detail => Detail.Student)
                .WithOne(Student => Student.StudentDetail)
                .HasForeignKey<StudentDetailEntity>(Detail => Detail.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
