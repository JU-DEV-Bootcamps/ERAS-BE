using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Views;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class ErasCalculationByPollConfiguration : IEntityTypeConfiguration<ErasCalculationsByPollEntity>
{
    public void Configure(EntityTypeBuilder<ErasCalculationsByPollEntity> builder)
    {
        builder.ToView("verascalculationbypoll");

        builder.HasNoKey();

        builder.Property(a => a.PollId)
            .HasColumnName("poll_id");

        builder.Property(a => a.PollUuid)
            .HasColumnName("poll_uuid");

        builder.Property(a => a.ComponentId)
            .HasColumnName("component_id");

        builder.Property(a => a.ComponentName)
            .HasColumnName("component_name");

        builder.Property(a => a.PollVariableId)
            .HasColumnName("poll_variable_id");

        builder.Property(a => a.Question)
            .HasColumnName("question");

        builder.Property(a => a.AnswerText)
            .HasColumnName("answer_text");

        builder.Property(a => a.PollInstanceId)
            .HasColumnName("poll_instance_id");

        builder.Property(a => a.PollInstanceRiskSum)
            .HasColumnName("poll_instance_risk_sum");

        builder.Property(a => a.Name)
            .HasColumnName("name");

        builder.Property(a => a.RiskSum)
            .HasColumnName("answer_risk");

        builder.Property(a => a.RiskCount)
            .HasColumnName("risk_count");

        builder.Property(a => a.AverageRisk)
            .HasColumnName("component_average_risk");

        builder.Property(a => a.VariableAverageRisk)
            .HasColumnName("variable_average_risk");

        builder.Property(a => a.AnswerCount)
            .HasColumnName("poll_instance_answers_count");

        builder.Property(a => a.Percentage)
            .HasColumnName("answer_percentage");

        builder.Property(a => a.CohortId)
            .HasColumnName("cohort_id");

        builder.Property(a => a.CohortName)
            .HasColumnName("cohort_name");

        builder.Property(a => a.AverageRiskByCohortComponent)
            .HasColumnName("average_risk_by_cohort_component");
    }
}

