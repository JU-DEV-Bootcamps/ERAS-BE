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
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                EvaluationPollId = dto.EvaluationPollId,
                Audit = dto.Audit,
            };
        }
        public static EvaluationDTO ToDto(this Evaluation domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            return new EvaluationDTO
            {
                Id = domain.Id,
                Name = domain.Name,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate,
                EvaluationPollId = domain.EvaluationPollId,
                Audit = domain.Audit,
            };
        }
    }
}
