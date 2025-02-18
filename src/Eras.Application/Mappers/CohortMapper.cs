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
    public static class CohortMapper
    {
        public static Cohort ToDomain(this CohortDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            return new Cohort
            {
                Name = dto.Name,
                CourseCode = dto.CourseCode,
                Audit = dto.Audit,
            };
        }
        public static CohortDTO ToDto(this Cohort domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            return new CohortDTO
            {
                Name = domain.Name,
                CourseCode = domain.CourseCode,
                Audit = domain.Audit,
            };
        }
    }
}
