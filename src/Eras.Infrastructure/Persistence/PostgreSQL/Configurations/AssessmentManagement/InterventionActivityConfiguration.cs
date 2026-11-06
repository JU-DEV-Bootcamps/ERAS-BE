using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public sealed class InterventionActivityConfiguration : IEntityTypeConfiguration<InterventionActivity>
{
    public void Configure(EntityTypeBuilder<InterventionActivity> Builder)
    {
        Builder.ToTable("intervention_activities");
        Builder.HasKey(Activity => Activity.Id);

        Builder.Property(Activity => Activity.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        Builder.Property(Activity => Activity.InterventionId).HasColumnName("intervention_id").IsRequired();

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

        Builder.HasIndex(Activity => Activity.InterventionId).HasDatabaseName("ix_intervention_activities_intervention_id");

        Builder.HasOne<Intervention>()
            .WithMany()
            .HasForeignKey(Activity => Activity.InterventionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
