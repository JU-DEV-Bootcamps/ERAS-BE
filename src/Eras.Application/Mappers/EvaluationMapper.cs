using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Dtos;
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
                StartDate = domain.StartDate,
                EndDate = domain.EndDate,
                Status = domain.Status,
                EvaluationPollId = domain.EvaluationPollId
            };
        }
    }
}
