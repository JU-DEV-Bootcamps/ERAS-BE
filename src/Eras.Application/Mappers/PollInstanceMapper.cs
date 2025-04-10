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
    public static class PollInstanceMapper
    {
        public static PollInstance ToDomain(this PollInstanceDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ICollection<Answer> answers = dto.Answers?.Select(ans => ans.ToDomain()).ToList() ?? [];
            return new PollInstance()
            {
                Uuid = dto.Uuid,
                Student = dto.Student?.ToDomain() ?? new Student(),
                Answers = answers,
                Audit = dto.Audit,
                FinishedAt = dto.FinishedAt
            };
        }
        public static PollInstanceDTO ToDTO(this PollInstance entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ICollection<AnswerDTO> answers = entity.Answers?.Select(ans => ans.ToDto()).ToList() ?? [];
            return new PollInstanceDTO()
            {
                Uuid = entity.Uuid,
                Student = entity.Student?.ToDto() ?? new StudentDTO(),
                Answers = answers,
                Audit = entity.Audit,
                FinishedAt = entity.FinishedAt
            };

        }
    }
}
