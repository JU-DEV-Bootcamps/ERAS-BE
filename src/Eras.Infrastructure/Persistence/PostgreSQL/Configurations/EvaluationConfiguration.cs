using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    internal class EvaluationConfiguration : IEntityTypeConfiguration<EvaluationEntity>
    {
        public void Configure(EntityTypeBuilder<EvaluationEntity> builder)
        {
            builder.ToTable("evaluation");

            ConfigureColumns(builder);
            AuditConfiguration.Configure(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<EvaluationEntity> builder)
        {
            builder.HasKey(evaluation => evaluation.Id);
            builder.Property(evaluation => evaluation.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(evaluation => evaluation.Status)
                .HasColumnName("status")
                .IsRequired();
            builder.Property(evaluation => evaluation.StartDate)
                .HasColumnName("start_date")
                .IsRequired();
            builder.Property(evaluation => evaluation.EndDate)
                .HasColumnName("end_date")
                .IsRequired();
            builder.Property(evaluation => evaluation.PollName)
                .HasColumnName("poll_name")
                .IsRequired();
        }
    }
}
