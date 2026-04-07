using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public sealed class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.ToTable("student_profiles");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(entity => entity.StudentCode)
            .HasColumnName("student_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.SupportAndReferralHistory)
            .HasColumnName("support_and_referral_history")
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(entity => entity.CharacterizationOrCurrentContext)
            .HasColumnName("characterization_or_current_context")
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.HasIndex(entity => entity.StudentCode)
            .IsUnique();
    }
}