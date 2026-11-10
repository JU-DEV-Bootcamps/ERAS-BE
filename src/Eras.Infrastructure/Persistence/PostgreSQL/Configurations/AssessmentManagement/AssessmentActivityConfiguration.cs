using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public sealed class AssessmentActivityConfiguration : IEntityTypeConfiguration<AssessmentActivity>
{
    public void Configure(EntityTypeBuilder<AssessmentActivity> Builder)
    {
        Builder.ToTable("assessment_activities");
        Builder.HasKey(Activity => Activity.Id);

        Builder.Property(Activity => Activity.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        Builder.Property(Activity => Activity.AssessmentId).HasColumnName("assessment_id").IsRequired();

        Builder.Property(Activity => Activity.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        Builder.Property(Activity => Activity.PreviousStatus)
            .HasColumnName("previous_status")
            .HasConversion<string>()
            .HasMaxLength(30);

        Builder.Property(Activity => Activity.NewStatus)
            .HasColumnName("new_status")
            .HasConversion<string>()
            .HasMaxLength(30);

        Builder.Property(Activity => Activity.OccurredAtUtc).HasColumnName("occurred_at_utc").IsRequired();
        Builder.Property(Activity => Activity.ActorSub).HasColumnName("actor_sub").HasMaxLength(200).IsRequired();
        Builder.Property(Activity => Activity.ActorName).HasColumnName("actor_name").HasMaxLength(200).IsRequired();

        Builder.HasIndex(Activity => Activity.AssessmentId).HasDatabaseName("ix_assessment_activities_assessment_id");

        Builder.HasOne<Assessment>()
            .WithMany()
            .HasForeignKey(Activity => Activity.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
