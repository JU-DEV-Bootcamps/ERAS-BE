using Eras.Domain.Entities.RemissionsManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.RemissionManagement;

public sealed class IndividualInterventionConfiguration : IEntityTypeConfiguration<IndividualIntervention>
{
    public void Configure(EntityTypeBuilder<IndividualIntervention> builder)
    {
    }
}