using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;

public sealed class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> Builder)
    {
        Builder.ToTable("student_profiles");
        ConfigureColumns(Builder);
        ConfigureRelationships(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<StudentProfile> Builder)
    {
        Builder.HasKey(Profile => Profile.Id);

        Builder.Property(Profile => Profile.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        Builder.Property(Profile => Profile.StudentId).HasColumnName("student_id").IsRequired();

        Builder.Property(Profile => Profile.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        Builder.Property(Profile => Profile.MiddleName).HasColumnName("middle_name").HasMaxLength(100);
        Builder.Property(Profile => Profile.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        Builder.Property(Profile => Profile.DateOfBirth).HasColumnName("date_of_birth");
        Builder.Property(Profile => Profile.Gender).HasColumnName("gender").HasMaxLength(50);
        Builder.Property(Profile => Profile.Nationality).HasColumnName("nationality").HasMaxLength(100);
        Builder.Property(Profile => Profile.CountryOfBirth).HasColumnName("country_of_birth").HasMaxLength(100);
        Builder.Property(Profile => Profile.IdPassportNumber).HasColumnName("id_passport_number").HasMaxLength(50).IsRequired();

        Builder.Property(Profile => Profile.SecondaryEmail).HasColumnName("secondary_email").HasMaxLength(255);
        Builder.Property(Profile => Profile.MobileNumber).HasColumnName("mobile_number").HasMaxLength(30);
        Builder.Property(Profile => Profile.Country).HasColumnName("country").HasMaxLength(100);
        Builder.Property(Profile => Profile.StateProvince).HasColumnName("state_province").HasMaxLength(100);
        Builder.Property(Profile => Profile.City).HasColumnName("city").HasMaxLength(100);
        Builder.Property(Profile => Profile.Street).HasColumnName("street").HasMaxLength(200);
        Builder.Property(Profile => Profile.PostalCode).HasColumnName("postal_code").HasMaxLength(20);

        Builder.Property(Profile => Profile.LevelOfStudy).HasColumnName("level_of_study").HasMaxLength(100);
        Builder.Property(Profile => Profile.FacultySchool).HasColumnName("faculty_school").HasMaxLength(150);
        Builder.Property(Profile => Profile.StudyModality).HasColumnName("study_modality").HasMaxLength(100);
        Builder.Property(Profile => Profile.PreviousInstitution).HasColumnName("previous_institution").HasMaxLength(200);

        Builder.HasIndex(Profile => Profile.StudentId)
            .IsUnique()
            .HasDatabaseName("ux_student_profiles_student_id");
        Builder.HasIndex(Profile => Profile.IdPassportNumber)
            .IsUnique()
            .HasDatabaseName("ux_student_profiles_id_passport_number");
    }

    private static void ConfigureRelationships(EntityTypeBuilder<StudentProfile> Builder)
    {
        Builder.HasOne<StudentEntity>()
            .WithOne()
            .HasForeignKey<StudentProfile>(Profile => Profile.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
