using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    internal class EvaluationConfiguration : IEntityTypeConfiguration<EvaluationEntity>
    {
        public void Configure(EntityTypeBuilder<EvaluationEntity> Builder)
        {
            Builder.ToTable("evaluation");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<EvaluationEntity> Builder)
        {
            Builder.HasKey(Evaluation => Evaluation.Id);
            Builder.Property(Evaluation => Evaluation.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();
            Builder.Property(Evaluation => Evaluation.Status)
                .HasColumnName("status")
                .HasMaxLength(30)
                .IsRequired();
            Builder.Property(Evaluation => Evaluation.StartDate)
                .HasColumnName("start_date")
                .IsRequired();
            Builder.Property(Evaluation => Evaluation.EndDate)
                .HasColumnName("end_date")
                .IsRequired();
            Builder.Property(Evaluation => Evaluation.PollName)
                .HasColumnName("poll_name")
                .HasMaxLength(100);
            Builder.Property(Evaluation => Evaluation.Country)
                .HasColumnName("country")
                .HasMaxLength(10);
            Builder.Property(e => e.ConfigurationId)
             .HasColumnName("configuration_id")
             .IsRequired();

        }

        private static void ConfigureRelationShips(EntityTypeBuilder<EvaluationEntity> Builder)
        {
            Builder.HasOne(e => e.Configuration)
                .WithMany()
                .HasForeignKey(e => e.ConfigurationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
