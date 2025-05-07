using Eras.Application.Dtos;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
namespace Eras.Application.Tests.Mappers;
public class AnswerMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_AnswerDTO_To_Answer()
    {
        var dto = new AnswerDTO
        {
            Answer = "Answer",
            Score = 1,
            PollInstanceId = 10,
            PollVariableId = 20
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Answer, result.AnswerText);
        Assert.Equal((int)dto.Score, result.RiskLevel);
        Assert.Equal(dto.PollInstanceId, result.PollInstanceId);
        Assert.Equal(dto.PollVariableId, result.PollVariableId);
    }

    [Fact]
    public void ToDto_Should_Convert_Answer_To_AnswerDto()
    {
        var model = new Answer
        {
            Id = 1,
            RiskLevel = 1,
            AnswerText = "This is an answer.",
            PollInstanceId = 10,
            PollVariableId = 20
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.RiskLevel, (int)result.Score);
        Assert.Equal(model.AnswerText, result.Answer);
        Assert.Equal(model.Audit, result.Audit);
        Assert.Equal(model.PollInstanceId, result.PollInstanceId);
        Assert.Equal(model.PollVariableId, result.PollVariableId);
    }
}
