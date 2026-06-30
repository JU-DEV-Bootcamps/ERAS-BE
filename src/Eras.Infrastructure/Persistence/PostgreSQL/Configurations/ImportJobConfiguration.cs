using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class ImportJobConfiguration : IEntityTypeConfiguration<ImportJobEntity>
    {
        public void Configure(EntityTypeBuilder<ImportJobEntity> Builder)
        {
            Builder.ToTable("import_jobs");
            Builder.HasKey(Job => Job.Id);

            Builder.Property(Job => Job.EvaluationId)
                .HasColumnName("evaluation_id")
                .IsRequired();
            Builder.Property(Job => Job.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            Builder.Property(Job => Job.TotalCount)
                .HasColumnName("total_count");
            Builder.Property(Job => Job.ProcessedCount)
                .HasColumnName("processed_count");
            Builder.Property(Job => Job.ExtractedCount)
                .HasColumnName("extracted_count");
            Builder.Property(Job => Job.RetryCount)
                .HasColumnName("retry_count");
            Builder.Property(Job => Job.ErrorMessage)
                .HasColumnName("error_message");
            Builder.Property(Job => Job.EvaluationSetName)
                .HasColumnName("evaluation_set_name")
                .HasMaxLength(200);
            Builder.Property(Job => Job.ConfigurationId)
                .HasColumnName("configuration_id");
            Builder.Property(Job => Job.StartDate)
                .HasColumnName("start_date")
                .HasMaxLength(40);
            Builder.Property(Job => Job.EndDate)
                .HasColumnName("end_date")
                .HasMaxLength(40);
            Builder.Property(Job => Job.PollsPayload)
                .HasColumnName("polls_payload")
                .HasColumnType("jsonb")
                .IsRequired();
            Builder.Property(Job => Job.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();
            Builder.Property(Job => Job.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .IsRequired();

            Builder.HasIndex(Job => Job.EvaluationId)
                .HasDatabaseName("ix_import_jobs_evaluation_id");
        }
    }
}
