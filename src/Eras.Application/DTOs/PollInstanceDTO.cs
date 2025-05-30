﻿using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class PollInstanceDTO
{
    public int Id { get; set; }
    public string Uuid { get; set; } = string.Empty;
    public StudentDTO Student { get; set; } = default!;
    public ICollection<AnswerDTO> Answers { get; set; } = [];
    public AuditInfo Audit { get; set; } = default!;
    public DateTime FinishedAt { get; set; }
    public int LastVersion { get; set; }
    public DateTime LastVersionDate { get; set; }
}
