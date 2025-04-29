using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class ErasCalculationByPollConfiguration : IEntityTypeConfiguration<ErasCalculationsByPollEntity>
{
    public void Configure(EntityTypeBuilder<ErasCalculationsByPollEntity> Builder)
    {
        Builder.ToView("verascalculationbypoll");

        Builder.HasNoKey();

        Builder.Property(A => A.PollUuid)
            .HasColumnName("poll_uuid");

        Builder.Property(A => A.ComponentName)
            .HasColumnName("component_name");

        Builder.Property(A => A.PollVariableId)
            .HasColumnName("poll_variable_id");

        Builder.Property(A => A.Question)
            .HasColumnName("question");

        Builder.Property(A => A.AnswerText)
            .HasColumnName("answer_text");

        Builder.Property(A => A.PollInstanceId)
            .HasColumnName("poll_instance_id");

        Builder.Property(A => A.StudentName)
            .HasColumnName("student_name");

        Builder.Property(A => A.StudentEmail)
            .HasColumnName("student_email");

        Builder.Property(A => A.AnswerRisk)
            .HasColumnName("answer_risk");

        Builder.Property(A => A.PollInstanceRiskSum)
            .HasColumnName("poll_instance_risk_sum");

        Builder.Property(A => A.PollInstanceAnswersCount)
            .HasColumnName("poll_instance_answers_count");

        Builder.Property(A => A.ComponentAverageRisk)
            .HasColumnName("component_average_risk");

        Builder.Property(A => A.VariableAverageRisk)
            .HasColumnName("variable_average_risk");

        Builder.Property(A => A.AnswerCount)
            .HasColumnName("answer_count");

        Builder.Property(A => A.AnswerPercentage)
            .HasColumnName("answer_percentage");


    }
}
