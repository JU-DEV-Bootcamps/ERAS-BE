using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;

public class ErasEvaluationDetailsViewConfiguration : IEntityTypeConfiguration<ErasEvaluationDetailsViewEntity>
{
    public void Configure(EntityTypeBuilder<ErasEvaluationDetailsViewEntity> Builder)
    {
        
        Builder.ToView("verasevaluationdetails");

        Builder.HasNoKey();

        Builder.Property(A => A.EvaluationId)
            .HasColumnName("evaluationid");

        Builder.Property(A => A.EvaluationName)
            .HasColumnName("evaluationname");

        Builder.Property(A => A.StartDate)
            .HasColumnName("startdate");

        Builder.Property(A => A.EndDate)
            .HasColumnName("enddate");

        Builder.Property(A => A.Status)
            .HasColumnName("status");

        Builder.Property(A => A.PollId)
            .HasColumnName("pollid");

        Builder.Property(A => A.PollName)
            .HasColumnName("pollname");

        Builder.Property(A => A.PollUuid)
            .HasColumnName("polluuid");

        Builder.Property(A => A.PollInstanceId)
            .HasColumnName("pollinstanceid");

        Builder.Property(A => A.FinishedAt)
            .HasColumnName("FinishedAt");

        Builder.Property(A => A.StudentId)
            .HasColumnName("studentid");

        Builder.Property(A => A.StudentName)
            .HasColumnName("studentname");

        Builder.Property(A => A.StudentEmail)
            .HasColumnName("studentemail");

        Builder.Property(A => A.CohortId)
            .HasColumnName("cohortid");

        Builder.Property(A => A.AnswerId)
            .HasColumnName("answerid");

        Builder.Property(A => A.AnswerText)
            .HasColumnName("answertext");

        Builder.Property(A => A.RiskLevel)
            .HasColumnName("risklevel");

        Builder.Property(A => A.VariableId)
            .HasColumnName("variableid");

        Builder.Property(A => A.VariableName)
            .HasColumnName("variablename");

        Builder.Property(A => A.ComponentId)
            .HasColumnName("componentid");

        Builder.Property(A => A.ComponentName)
            .HasColumnName("componentname");

        Builder.Property(A => A.VariableVersion)
            .HasColumnName("variableversion");
    }
}
