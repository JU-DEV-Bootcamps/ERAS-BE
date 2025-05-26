using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class EvaluationMapper
    {
        public static Evaluation ToDomain(this EvaluationDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            return new Evaluation
            {
                Id = dto.Id,
                Name = dto.Name,
                PollName = dto.PollName,
                Country = dto.Country,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EvaluationPollId = dto.EvaluationPollId
            };
        }
        public static EvaluationDTO ToDto(this Evaluation domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            return new EvaluationDTO
            {
                Id = domain.Id,
                Name = domain.Name,
                PollName = domain.PollName,
                Country = domain.Country,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate,
                Status = domain.Status,
                EvaluationPollId = domain.EvaluationPollId
            };
        }
    }
}
