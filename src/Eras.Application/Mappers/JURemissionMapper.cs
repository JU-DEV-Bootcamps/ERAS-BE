using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class JURemissionMapper
    {
        public static JURemission ToDomain(this JURemissionDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);

            return new JURemission()
            {
                Id = Dto.Id,
                SubmitterUuid = Dto.SubmitterUuid,
                JUService = Dto.JUService.ToDomain(),
                AssignedProfessional = Dto.AssignedProfessional.ToDomain(),
                Comment = Dto.Comment,
                Date = Dto.Date,
                Status = Dto.Status,
                Students = Dto.Students.Select(Stu => Stu.ToDomain()).ToList(),
                Audit = Dto.Audit,
            };
        }
        public static JURemissionDTO ToDTO(this JURemission Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);
            return new JURemissionDTO()
            {
                Id = Entity.Id,
                SubmitterUuid = Entity.SubmitterUuid,
                JUService = Entity.JUService.ToDTO(),
                AssignedProfessional = Entity.AssignedProfessional.ToDTO(),
                Comment = Entity.Comment,
                Date = Entity.Date,
                Status = Entity.Status,
                Students = Entity.Students.Select(Stu => Stu.ToDto()).ToList(),
                Audit = Entity.Audit,
            };

        }
    }
}
