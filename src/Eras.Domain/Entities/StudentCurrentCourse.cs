namespace Eras.Domain.Entities;

/// <summary>A course the student is currently taking, stored as JSON inside <see cref="StudentProfile"/>.</summary>
public class StudentCurrentCourse
{
    public string Name { get; set; } = string.Empty;
    public string? Group { get; set; }
    public string? Section { get; set; }
}
