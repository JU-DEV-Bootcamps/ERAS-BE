using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public sealed class IndividualInterventionConfiguration : IEntityTypeConfiguration<IndividualIntervention>
{
    public void Configure(EntityTypeBuilder<IndividualIntervention> builder)
    {
    }
}