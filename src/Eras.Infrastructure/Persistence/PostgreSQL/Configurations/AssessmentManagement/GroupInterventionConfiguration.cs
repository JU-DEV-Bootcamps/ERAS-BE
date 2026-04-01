using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public sealed class GroupInterventionConfiguration : IEntityTypeConfiguration<GroupIntervention>
{
    public void Configure(EntityTypeBuilder<GroupIntervention> builder)
    {
        builder.Property(entity => entity.Area)
            .HasColumnName("area")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(entity => entity.ParticipantIds)
            .HasColumnName("participant_ids")
            .HasColumnType("uuid[]")
            .IsRequired();
    }
}
