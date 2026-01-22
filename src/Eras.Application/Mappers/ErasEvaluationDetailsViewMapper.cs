using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Application.DTOs.Views;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers;

public static class ErasEvaluationDetailsViewMapper
{
    public static ErasEvaluationDetailsView ToDomain(this ErasEvaluationDetailsViewDTO Dto)
    {
        ArgumentNullException.ThrowIfNull(Dto);
        return new ErasEvaluationDetailsView
        {
            EvaluationId = Dto.EvaluationId,
            EvaluationName = Dto.EvaluationName,
            StartDate = Dto.StartDate,
            EndDate = Dto.EndDate,
            Status = Dto.Status,
            PollId = Dto.PollId,
            PollName = Dto.PollName,
            PollUuid = Dto.PollUuid,
            PollInstanceId = Dto.PollInstanceId,
            FinishedAt = Dto.FinishedAt,
            StudentId = Dto.StudentId,
            StudentName = Dto.StudentName,
            StudentEmail = Dto.StudentEmail,
            CohortId = Dto.CohortId,
            AnswerId = Dto.AnswerId,
            AnswerText = Dto.AnswerText,
            RiskLevel = Dto.RiskLevel,
            VariableId = Dto.VariableId,
            VariableName = Dto.VariableName,
            ComponentId = Dto.ComponentId,
            ComponentName = Dto.ComponentName,
            VariableVersion = Dto.VariableVersion,
        };
    }
    public static ErasEvaluationDetailsViewDTO ToDto(this ErasEvaluationDetailsView Domain)
    {
        ArgumentNullException.ThrowIfNull(Domain);
        return new ErasEvaluationDetailsViewDTO
        {
            EvaluationId = Domain.EvaluationId,
            EvaluationName = Domain.EvaluationName,
            StartDate = Domain.StartDate,
            EndDate = Domain.EndDate,
            Status = Domain.Status,
            PollId = Domain.PollId,
            PollName = Domain.PollName,
            PollUuid = Domain.PollUuid,
            PollInstanceId = Domain.PollInstanceId,
            FinishedAt = Domain.FinishedAt,
            StudentId = Domain.StudentId,
            StudentName = Domain.StudentName,
            StudentEmail = Domain.StudentEmail,
            CohortId = Domain.CohortId,
            AnswerId = Domain.AnswerId,
            AnswerText = Domain.AnswerText,
            RiskLevel = Domain.RiskLevel,
            VariableId = Domain.VariableId,
            VariableName = Domain.VariableName,
            ComponentId = Domain.ComponentId,
            ComponentName = Domain.ComponentName,
            VariableVersion = Domain.VariableVersion,
        };
    }
}
