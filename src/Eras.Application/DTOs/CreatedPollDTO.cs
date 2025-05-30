﻿namespace Eras.Application.DTOs;

public class CreatedPollDTO
{
    public int Id { get; set; }
    public string IdCosmicLatte { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime FinishedAt { get; set; }
    public ICollection<StudentDTO> studentDTOs { get; set; } = [];
}