using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using static Eras.Domain.Entities.JURemissionsConstants;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class JURemissionConfiguration : IEntityTypeConfiguration<JURemissionEntity>
{
    public void Configure(EntityTypeBuilder<JURemissionEntity> Builder)
    {
        Builder.ToTable("ju_remissions");
        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<JURemissionEntity> Builder)
    {
        Builder.HasKey(R => R.Id);

        Builder.Property(R => R.SubmitterUuid)
            .HasColumnName("submitter_uuid")
            .HasMaxLength(100)
            .IsRequired();

        Builder.Property(R => R.AssignedProfessionalUuid)
            .HasColumnName("assigned_professional_uuid")
            .HasMaxLength(100)
            .IsRequired();

        Builder.Property(R => R.Comment)
            .HasColumnName("comment")
            .HasMaxLength(1000);

        Builder.Property(R => R.Date)
            .HasColumnName("date")
            .IsRequired();

        Builder.Property(R => R.Status)
            .HasColumnName("status")
            .HasConversion(
                V => V.ToString(),
                V => (RemissionsStatus)Enum.Parse(typeof(RemissionsStatus), V))
            .HasMaxLength(50)
            .IsRequired();

        Builder.Property(R => R.StudentId)
            .HasColumnName("student_id")
            .IsRequired();

        Builder.Property(R => R.JUServiceId)
            .HasColumnName("ju_service_id")
            .IsRequired();
    }
    private void ConfigureRelationShips(EntityTypeBuilder<JURemissionEntity> Builder)
    {
        Builder.HasMany(R => R.Students)
            .WithMany(S => S.Remissions)
            .UsingEntity(J => J.ToTable("student_remissions"));

        Builder.HasOne(R => R.JUService)
            .WithMany()
            .HasForeignKey(R => R.JUServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
