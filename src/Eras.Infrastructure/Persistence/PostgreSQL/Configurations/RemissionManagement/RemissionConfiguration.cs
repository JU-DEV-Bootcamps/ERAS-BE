using Eras.Domain.Entities.RemissionsManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.RemissionManagement;

public sealed class RemissionConfiguration : IEntityTypeConfiguration<Remission>
{
    public void Configure(EntityTypeBuilder<Remission> builder)
    {
        builder.ToTable("remissions");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(entity => entity.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        UtcDateTimeConfiguration.Configure(
            builder.Property(entity => entity.CreatedAtUtc));

        builder.Property(entity => entity.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.Service)
            .HasColumnName("service")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.AssignedProfessional)
            .HasColumnName("assigned_professional")
            .HasMaxLength(200)
            .IsRequired(false);

        JsonCollectionConfiguration.ConfigureGuidCollection(
            builder.Property(entity => entity.StudentIds)
                .HasColumnName("student_ids"))
            .IsRequired();

        builder.Property(entity => entity.Diagnosis)
            .HasColumnName("diagnosis")
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(entity => entity.Objective)
            .HasColumnName("objective")
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(entity => entity.Comments)
            .HasColumnName("comments")
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(entity => entity.Plan, owned =>
        {
            owned.Property(plan => plan.Id)
                .HasColumnName("plan_id")
                .IsRequired(false);

            owned.Property(plan => plan.SessionsPerWeek)
                .HasColumnName("plan_sessions_per_week")
                .IsRequired(false);

            owned.Property(plan => plan.ScheduleNotes)
                .HasColumnName("plan_schedule_notes")
                .HasMaxLength(2000)
                .IsRequired(false);
        });

        builder.Navigation(entity => entity.Plan)
            .IsRequired(false);

        builder.HasMany(entity => entity.Interventions)
            .WithOne()
            .HasForeignKey("remission_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}