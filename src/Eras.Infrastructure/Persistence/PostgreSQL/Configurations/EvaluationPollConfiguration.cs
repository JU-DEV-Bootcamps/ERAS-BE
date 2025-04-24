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
        public void Configure(EntityTypeBuilder<EvaluationPollJoin> Builder)
        {
            Builder.ToTable("evaluation_poll");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<EvaluationPollJoin> Builder)
        {
            Builder.HasOne(EvaluationPoll => EvaluationPoll.Poll)
                .WithMany(Poll => Poll.EvaluationPolls)
                .HasForeignKey(EvaluationPoll => EvaluationPoll.PollId)
                .OnDelete(DeleteBehavior.Cascade);
            Builder.HasOne(EvaluationPoll => EvaluationPoll.Evaluation)
                .WithMany(Evaluation => Evaluation.EvaluationPolls)
                .HasForeignKey(EvaluationPoll => EvaluationPoll.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureColumns(EntityTypeBuilder<EvaluationPollJoin> Builder)
        {
            Builder.HasKey(EvaluationPoll => EvaluationPoll.Id);
            Builder.Property(EvaluationPoll => EvaluationPoll.PollId)
                .HasColumnName("poll_id")
                .IsRequired();
            Builder.Property(EvaluationPoll => EvaluationPoll.EvaluationId)
                .HasColumnName("evaluation_id")
                .IsRequired();
        }
    }
}
