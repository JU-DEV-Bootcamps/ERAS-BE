using Eras.Application.DTOs.Views;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;

public class ErasEvaluationDetailsViewMapperTest
{
    [Fact]
    public void ToDomain_Should_Map_All_Properties()
    {
        var dto = new ErasEvaluationDetailsViewDTO
        {
            EvaluationId = 1,
            EvaluationName = "Evaluation",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            Status = "Completed",
            PollId = 2,
            PollName = "Poll",
            PollUuid = "poll-uuid",
            PollInstanceId = 3,
            FinishedAt = DateTime.UtcNow,
            StudentId = 4,
            StudentName = "Student",
            StudentEmail = "student@test.com",
            CohortId = 5,
            AnswerId = 6,
            AnswerText = "Answer",
            RiskLevel = 1,
            VariableId = 7,
            VariableName = "Variable",
            ComponentId = 8,
            ComponentName = "Component",
            VariableVersion = 9
        };

        var result = dto.ToDomain();

        Assert.Equal(dto.EvaluationId, result.EvaluationId);
        Assert.Equal(dto.EvaluationName, result.EvaluationName);
        Assert.Equal(dto.StartDate, result.StartDate);
        Assert.Equal(dto.EndDate, result.EndDate);
        Assert.Equal(dto.Status, result.Status);
        Assert.Equal(dto.PollId, result.PollId);
        Assert.Equal(dto.PollName, result.PollName);
        Assert.Equal(dto.PollUuid, result.PollUuid);
        Assert.Equal(dto.PollInstanceId, result.PollInstanceId);
        Assert.Equal(dto.FinishedAt, result.FinishedAt);
        Assert.Equal(dto.StudentId, result.StudentId);
        Assert.Equal(dto.StudentName, result.StudentName);
        Assert.Equal(dto.StudentEmail, result.StudentEmail);
        Assert.Equal(dto.CohortId, result.CohortId);
        Assert.Equal(dto.AnswerId, result.AnswerId);
        Assert.Equal(dto.AnswerText, result.AnswerText);
        Assert.Equal(dto.RiskLevel, result.RiskLevel);
        Assert.Equal(dto.VariableId, result.VariableId);
        Assert.Equal(dto.VariableName, result.VariableName);
        Assert.Equal(dto.ComponentId, result.ComponentId);
        Assert.Equal(dto.ComponentName, result.ComponentName);
        Assert.Equal(dto.VariableVersion, result.VariableVersion);
    }

    [Fact]
    public void ToDomain_Should_Throw_When_Dto_Is_Null()
    {
        ErasEvaluationDetailsViewDTO dto = null!;

        Assert.Throws<ArgumentNullException>(() => dto.ToDomain());
    }

    [Fact]
    public void ToDto_Should_Map_All_Properties()
    {
        var domain = new ErasEvaluationDetailsView
        {
            EvaluationId = 1,
            EvaluationName = "Evaluation",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow,
            Status = "Completed",
            PollId = 2,
            PollName = "Poll",
            PollUuid = "poll-uuid",
            PollInstanceId = 3,
            FinishedAt = DateTime.UtcNow,
            StudentId = 4,
            StudentName = "Student",
            StudentEmail = "student@test.com",
            CohortId = 5,
            AnswerId = 6,
            AnswerText = "Answer",
            RiskLevel = 1,
            VariableId = 7,
            VariableName = "Variable",
            ComponentId = 8,
            ComponentName = "Component",
            VariableVersion = 9
        };

        var result = domain.ToDto();

        Assert.Equal(domain.EvaluationId, result.EvaluationId);
        Assert.Equal(domain.EvaluationName, result.EvaluationName);
        Assert.Equal(domain.StartDate, result.StartDate);
        Assert.Equal(domain.EndDate, result.EndDate);
        Assert.Equal(domain.Status, result.Status);
        Assert.Equal(domain.PollId, result.PollId);
        Assert.Equal(domain.PollName, result.PollName);
        Assert.Equal(domain.PollUuid, result.PollUuid);
        Assert.Equal(domain.PollInstanceId, result.PollInstanceId);
        Assert.Equal(domain.FinishedAt, result.FinishedAt);
        Assert.Equal(domain.StudentId, result.StudentId);
        Assert.Equal(domain.StudentName, result.StudentName);
        Assert.Equal(domain.StudentEmail, result.StudentEmail);
        Assert.Equal(domain.CohortId, result.CohortId);
        Assert.Equal(domain.AnswerId, result.AnswerId);
        Assert.Equal(domain.AnswerText, result.AnswerText);
        Assert.Equal(domain.RiskLevel, result.RiskLevel);
        Assert.Equal(domain.VariableId, result.VariableId);
        Assert.Equal(domain.VariableName, result.VariableName);
        Assert.Equal(domain.ComponentId, result.ComponentId);
        Assert.Equal(domain.ComponentName, result.ComponentName);
        Assert.Equal(domain.VariableVersion, result.VariableVersion);
    }

    [Fact]
    public void ToDto_Should_Throw_When_Domain_Is_Null()
    {
        ErasEvaluationDetailsView domain = null!;

        Assert.Throws<ArgumentNullException>(() => domain.ToDto());
    }
}
