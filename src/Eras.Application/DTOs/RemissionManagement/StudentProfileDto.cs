using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.DTOs.RemissionManagement;

public sealed record StudentProfileDto
{
    public Guid? Id { get; init; }

    public required string StudentCode { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public string? SupportAndReferralHistory { get; init; }
    public string? CharacterizationOrCurrentContext { get; init; }
}
