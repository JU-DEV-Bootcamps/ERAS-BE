using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.ToTable("answers");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(answer => answer.Id);
            builder.Property(answer => answer.AnswerText)
                .HasColumnName("answer_text")
                .IsRequired();
            builder.Property(answer => answer.RiskLevel)
                .HasColumnName("risk_level")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<Answer> builder)
        {
            builder.HasOne(answer => answer.PollInstance)
                .WithMany(pollInstance => pollInstance.Answers)
                .HasForeignKey(answer => answer.PollInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}