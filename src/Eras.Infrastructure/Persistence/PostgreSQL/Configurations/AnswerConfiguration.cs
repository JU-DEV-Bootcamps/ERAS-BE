using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class AnswerConfiguration : IEntityTypeConfiguration<AnswerEntity>
    {
        public void Configure(EntityTypeBuilder<AnswerEntity> Builder)
        {
            Builder.ToTable("answers");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            ConfigureConstraints(Builder);
            VersionConfiguration.Configure(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<AnswerEntity> Builder)
        {
            Builder.HasKey(Answer => Answer.Id);
            Builder.Property(Answer => Answer.AnswerText)
                .HasColumnName("answer_text")
                .HasMaxLength(500)
                .IsRequired();
            Builder.Property(Answer => Answer.RiskLevel)
                .HasColumnName("risk_level")
                .HasPrecision(7,4)
                .IsRequired();
            Builder.Property(Answer => Answer.PollInstanceId)
                .HasColumnName("poll_instance_id")
                .IsRequired();
            Builder.Property(Answer => Answer.PollVariableId)
                .HasColumnName("poll_variable_id")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<AnswerEntity> Builder)
        {
            Builder.HasOne(Answer => Answer.PollInstance)
                .WithMany(PollInstance => PollInstance.Answers)
                .HasForeignKey(Answer => Answer.PollInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
            Builder.HasOne(Answer => Answer.PollVariable)
                .WithMany(PollVariable => PollVariable.Answers)
                .HasForeignKey(Answer => Answer.PollVariableId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureConstraints(EntityTypeBuilder<AnswerEntity> Builder)
        {
            Builder.HasAlternateKey(Answer => new { Answer.PollInstanceId, Answer.PollVariableId, Answer.AnswerText })
                .HasName("Unique_PollInstanceId_PollVariableId_AnswerText");
        }
    }
}
