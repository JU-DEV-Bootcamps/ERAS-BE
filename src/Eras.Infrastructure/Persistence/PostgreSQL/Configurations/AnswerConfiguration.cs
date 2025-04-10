using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class AnswerConfiguration : IEntityTypeConfiguration<AnswerEntity>
    {
        public void Configure(EntityTypeBuilder<AnswerEntity> builder)
        {
            builder.ToTable("answers");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            ConfigureConstraints(builder);
            AuditConfiguration.Configure(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<AnswerEntity> builder)
        {
            builder.HasKey(answer => answer.Id);
            builder.Property(answer => answer.AnswerText)
                .HasColumnName("answer_text")
                .IsRequired();
            builder.Property(answer => answer.RiskLevel)
                .HasColumnName("risk_level")
                .IsRequired();
            builder.Property(answer => answer.PollInstanceId)
                .HasColumnName("poll_instance_id")
                .IsRequired();
            builder.Property(answer => answer.PollVariableId)
                .HasColumnName("poll_variable_id")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<AnswerEntity> builder)
        {
            builder.HasOne(answer => answer.PollInstance)
                .WithMany(pollInstance => pollInstance.Answers)
                .HasForeignKey(answer => answer.PollInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(answer => answer.PollVariable)
                .WithMany(pollVariable => pollVariable.Answers)
                .HasForeignKey(answer => answer.PollVariableId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureConstraints(EntityTypeBuilder<AnswerEntity> builder)
        {
            builder.HasAlternateKey(answer => new { answer.PollInstanceId, answer.PollVariableId, answer.AnswerText })
                .HasName("Unique_PollInstanceId_PollVariableId_AnswerText");
        }
    }
}
