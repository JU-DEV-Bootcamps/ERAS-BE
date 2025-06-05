using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class CohortConfiguration : IEntityTypeConfiguration<CohortEntity>
    {
        public void Configure(EntityTypeBuilder<CohortEntity> Builder)
        {
            Builder.ToTable("cohorts");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<CohortEntity> Builder)
        {
            Builder.HasKey(Cohort => Cohort.Id);
            Builder.Property(Cohort => Cohort.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();
            Builder.Property(Cohort => Cohort.CourseCode)
                .HasColumnName("course_code")
                .HasMaxLength(50)
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<CohortEntity> Builder)
        {
        }
    }
}
