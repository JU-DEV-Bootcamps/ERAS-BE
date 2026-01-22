using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

public static class ErasEvaluationDetailsViewMapper
{
    public static Domain.Entities.ErasEvaluationDetailsView ToDomain(this ErasEvaluationDetailsViewEntity Entity)
    {
        return new Domain.Entities.ErasEvaluationDetailsView
        {
            
            EvaluationId = Entity.EvaluationId,
            EvaluationName = Entity.EvaluationName,
            StartDate = Entity.StartDate,
            EndDate = Entity.EndDate,
            Status = Entity.Status,
            PollId = Entity.PollId,
            PollName = Entity.PollName,
            PollUuid = Entity.PollUuid,
            PollInstanceId = Entity.PollInstanceId,
            FinishedAt = Entity.FinishedAt,
            StudentId = Entity.StudentId,
            StudentName = Entity.StudentName,
            StudentEmail = Entity.StudentEmail,
            CohortId = Entity.CohortId,
            AnswerId = Entity.AnswerId,
            AnswerText = Entity.AnswerText,
            RiskLevel = Entity.RiskLevel,
            VariableId = Entity.VariableId,
            VariableName = Entity.VariableName,
            ComponentId = Entity.ComponentId,
            ComponentName = Entity.ComponentName,
            VariableVersion = Entity.VariableVersion,
        };
    }

    public static ErasEvaluationDetailsViewEntity ToPersistence(this Domain.Entities.ErasEvaluationDetailsView Model)
    {
        return new ErasEvaluationDetailsViewEntity
        {
            EvaluationId = Model.EvaluationId,
            EvaluationName = Model.EvaluationName,
            StartDate = Model.StartDate,
            EndDate = Model.EndDate,
            Status = Model.Status,
            PollId = Model.PollId,
            PollName = Model.PollName,
            PollUuid = Model.PollUuid,
            PollInstanceId = Model.PollInstanceId,
            FinishedAt = Model.FinishedAt,
            StudentId = Model.StudentId,
            StudentName = Model.StudentName,
            StudentEmail = Model.StudentEmail,
            CohortId = Model.CohortId,
            AnswerId = Model.AnswerId,
            AnswerText = Model.AnswerText,
            RiskLevel = Model.RiskLevel,
            VariableId = Model.VariableId,
            VariableName = Model.VariableName,
            ComponentId = Model.ComponentId,
            ComponentName = Model.ComponentName,
            VariableVersion = Model .VariableVersion,
        };
    }
}
