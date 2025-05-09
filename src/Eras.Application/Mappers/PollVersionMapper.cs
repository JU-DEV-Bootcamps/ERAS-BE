using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers;
public static class PollVersionMapper
{
    public static PollVersion ToDomain(this PollVersionDTO Dto) {
        return new PollVersion()
        {
            Id = Dto.Id,
            Name = Dto.Name,
            Date = Dto.Date,
            Audit = Dto.Audit,
        };
    }

    public static PollVersionDTO ToDto(this PollVersion Entity)
    {
        return new PollVersionDTO()
        {
            Id = Entity.Id,
            Name = Entity.Name,
            Date = Entity.Date,
            Audit = Entity.Audit,
        };
    }
}
