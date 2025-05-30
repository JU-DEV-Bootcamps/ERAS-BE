﻿using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class CohortMapper
    {
        public static Cohort ToDomain(this CohortDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);
            return new Cohort
            {
                Name = Dto.Name,
                CourseCode = Dto.CourseCode,
                Audit = Dto.Audit?? new AuditInfo()
                {
                    CreatedBy = "Cohort Mapper",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                },
            };
        }
        public static CohortDTO ToDto(this Cohort Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            return new CohortDTO
            {
                Name = Domain.Name,
                CourseCode = Domain.CourseCode,
                Audit = Domain.Audit,
            };
        }
    }
}
