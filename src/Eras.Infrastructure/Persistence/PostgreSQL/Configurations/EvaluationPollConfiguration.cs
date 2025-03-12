using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class EvaluationPollConfiguration : IEntityTypeConfiguration<EvaluationPollJoin>
    {
        public void Configure(EntityTypeBuilder<EvaluationPollJoin> builder)
        {
            builder.ToTable("evaluation_poll");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<EvaluationPollJoin> builder)
        {
            builder.HasOne(evaluationPoll => evaluationPoll.Poll)
                .WithMany(poll => poll.EvaluationPolls)
                .HasForeignKey(evaluationPoll => evaluationPoll.PollId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(evaluationPoll => evaluationPoll.Evaluation)
                .WithMany(evaluation => evaluation.EvaluationPolls)
                .HasForeignKey(evaluationPoll => evaluationPoll.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureColumns(EntityTypeBuilder<EvaluationPollJoin> builder)
        {
            builder.HasKey(evaluationPoll => evaluationPoll.Id);
            builder.Property(evaluationPoll => evaluationPoll.PollId)
                .HasColumnName("poll_id")
                .IsRequired();
            builder.Property(evaluationPoll => evaluationPoll.EvaluationId)
                .HasColumnName("evaluation_id")
                .IsRequired();
        }
    }
}
