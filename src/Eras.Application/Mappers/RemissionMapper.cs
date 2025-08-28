
using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class RemissionMapper
    {
        public static JURemission ToDomain(this JURemissionDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            ICollection<Answer> answers = Dto.Answers?.Select(Ans => Ans.ToDomain()).ToList() ?? [];
            return new JURemission ()
            {
                Id = Dto.Id,
                Uuid = Dto.Uuid,             
                
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
