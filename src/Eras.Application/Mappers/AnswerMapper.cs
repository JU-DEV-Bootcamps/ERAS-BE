using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class AnswerMapper
    {
        public static Answer ToDomain(this AnswerDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            return new Answer
            {
                AnswerText = Dto.Answer,
                RiskLevel = (int) Dto.Score,
                PollInstanceId = Dto.PollInstanceId,
                PollVariableId = Dto.PollVariableId,
                Audit = Dto.Audit?? new AuditInfo(),
                Version = Dto.Version,
            };
        }
        public static AnswerDTO ToDto(this Answer Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            return new AnswerDTO
            {
                Answer = Domain.AnswerText,
                Score = Domain.RiskLevel,
                PollInstanceId = Domain.PollInstanceId,
                PollVariableId = Domain.PollVariableId,
                Audit = Domain.Audit,
                Version = Domain.Version,
            };
        }
    }
}