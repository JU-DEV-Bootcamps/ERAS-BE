using Eras.Domain.Entities.UserManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;

public sealed class ProfessionalProfileConfiguration : IEntityTypeConfiguration<ProfessionalProfile>
{
    public void Configure(EntityTypeBuilder<ProfessionalProfile> Builder)
    {
        Builder.ToTable("professional_profiles");
        ConfigureColumns(Builder);
        ConfigureCourses(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<ProfessionalProfile> Builder)
    {
        Builder.HasKey(Profile => Profile.Id);

        Builder.Property(Profile => Profile.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        Builder.Property(Profile => Profile.ErasUserId)
            .HasColumnName("eras_user_id")
            .IsRequired();

        // One profile per user; deleting the user removes the profile.
        Builder.HasIndex(Profile => Profile.ErasUserId)
            .IsUnique()
            .HasDatabaseName("ux_professional_profiles_eras_user_id");

        Builder.HasOne<ErasUser>()
            .WithMany()
            .HasForeignKey(Profile => Profile.ErasUserId)
            .OnDelete(DeleteBehavior.Cascade);

        Builder.Property(Profile => Profile.Faculty)
            .HasColumnName("faculty")
            .HasMaxLength(200)
            .IsRequired();

        Builder.Property(Profile => Profile.OfficeHours)
            .HasColumnName("office_hours")
            .HasMaxLength(200)
            .IsRequired();

        // ADR-0002: a short list with no attributes of its own is stored in an array column.
        Builder.Property(Profile => Profile.SubjectsTaught)
            .HasColumnName("subjects_taught")
            .HasColumnType("text[]")
            .IsRequired();
    }

    private static void ConfigureCourses(EntityTypeBuilder<ProfessionalProfile> Builder)
    {
        // ADR-0002: a course has attributes of its own (code, name), so it is a child table.
        Builder.OwnsMany(Profile => Profile.Courses, Course =>
        {
            Course.ToTable("professional_courses");
            Course.WithOwner()
                .HasForeignKey("professional_profile_id")
                .HasConstraintName("FK_professional_courses_professional_profile");
            Course.HasKey(Entry => Entry.Id);

            Course.Property(Entry => Entry.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd()
                .UseIdentityByDefaultColumn();

            Course.Property(Entry => Entry.Code)
                .HasColumnName("code")
                .HasMaxLength(20)
                .IsRequired();

            Course.Property(Entry => Entry.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();
        });
    }
}
