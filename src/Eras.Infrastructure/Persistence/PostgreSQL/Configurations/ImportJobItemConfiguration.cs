using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class ImportJobItemConfiguration : IEntityTypeConfiguration<ImportJobItemEntity>
    {
        public void Configure(EntityTypeBuilder<ImportJobItemEntity> Builder)
        {
            Builder.ToTable("import_job_items");
            Builder.HasKey(Item => Item.Id);

            Builder.Property(Item => Item.ImportJobId)
                .HasColumnName("import_job_id")
                .IsRequired();
            Builder.Property(Item => Item.StudentEmail)
                .HasColumnName("student_email")
                .HasMaxLength(320)
                .IsRequired();
            Builder.Property(Item => Item.StudentName)
                .HasColumnName("student_name")
                .HasMaxLength(200)
                .IsRequired();
            Builder.Property(Item => Item.Cohort)
                .HasColumnName("cohort")
                .HasMaxLength(200);
            Builder.Property(Item => Item.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            Builder.Property(Item => Item.RetryCount)
                .HasColumnName("retry_count");
            Builder.Property(Item => Item.ErrorMessage)
                .HasColumnName("error_message");
            Builder.Property(Item => Item.PollPayload)
                .HasColumnName("poll_payload")
                .HasColumnType("jsonb")
                .IsRequired();
            Builder.Property(Item => Item.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();
            Builder.Property(Item => Item.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .IsRequired();

            Builder.HasIndex(Item => new { Item.ImportJobId, Item.Status })
                .HasDatabaseName("ix_import_job_items_import_job_id_status");
        }
    }
}
