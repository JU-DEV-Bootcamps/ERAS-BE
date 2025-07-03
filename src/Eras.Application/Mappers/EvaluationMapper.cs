using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class EvaluationMapper
    {
        public static Evaluation ToDomain(this EvaluationDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            return new Evaluation
            {
                Id = Dto.Id,
                Name = Dto.Name,
                PollName = Dto.PollName,
                Country = Dto.Country,
                StartDate = Dto.StartDate,
                EndDate = Dto.EndDate,
                EvaluationPollId = Dto.EvaluationPollId,
                ConfigurationId = Dto.ConfigurationId
            };
        }
        public static EvaluationDTO ToDto(this Evaluation Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            return new EvaluationDTO
            {
                Id = Domain.Id,
                Name = Domain.Name,
                PollName = Domain.PollName,
                Country = Domain.Country,
                StartDate = Domain.StartDate,
                EndDate = Domain.EndDate,
                Status = Domain.Status,
                EvaluationPollId = Domain.EvaluationPollId,
                ConfigurationId = Domain.ConfigurationId
            };
        }
    }
}
