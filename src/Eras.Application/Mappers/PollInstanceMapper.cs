using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Mappers
{
    public static class PollInstanceMapper
    {
        public static PollInstance ToDomain(this PollInstanceDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            ICollection<Answer> answers = Dto.Answers?.Select(Ans => Ans.ToDomain()).ToList() ?? [];
            return new PollInstance()
            {
                Id = Dto.Id,
                Uuid = Dto.Uuid,             
                Student = Dto.Student?.ToDomain() ?? new Student(),
                Answers = answers,
                Audit = Dto.Audit,
                FinishedAt = Dto.FinishedAt,
                LastVersion = Dto.LastVersion,
                LastVersionDate = Dto.LastVersionDate,
            };
        }
        public static PollInstanceDTO ToDTO(this PollInstance Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);
            ICollection<AnswerDTO> answers = Entity.Answers?.Select(Ans => Ans.ToDto()).ToList() ?? [];
            return new PollInstanceDTO()
            {
                Id = Entity.Id,
                Uuid = Entity.Uuid,
                Student = Entity.Student?.ToDto() ?? new StudentDTO(),
                Answers = answers,
                Audit = Entity.Audit,
                FinishedAt = Entity.FinishedAt,
                LastVersion = Entity.LastVersion,
                LastVersionDate = Entity.LastVersionDate,
            };

        }
    }
}
