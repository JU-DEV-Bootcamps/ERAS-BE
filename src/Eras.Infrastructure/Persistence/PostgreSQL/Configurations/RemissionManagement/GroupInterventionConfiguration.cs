using Eras.Domain.Entities.RemissionsManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.RemissionManagement;

public sealed class GroupInterventionConfiguration : IEntityTypeConfiguration<GroupIntervention>
{
    public void Configure(EntityTypeBuilder<GroupIntervention> builder)
    {
        builder.Property(entity => entity.Area)
            .HasColumnName("area")
            .HasMaxLength(200)
            .IsRequired(false);

        JsonCollectionConfiguration.ConfigureGuidCollection(
            builder.Property(entity => entity.ParticipantIds)
                .HasColumnName("participant_ids"))
            .IsRequired();
    }
}
