using Eras.Domain.Entities;

namespace Eras.Application.Models.Response.Controllers.StudentsController;
public class GetAllStudentsQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
    public bool IsImported { get; set; } = false;
    /// <summary>True for students registered through "New Student" (they have a profile to view/edit).</summary>
    public bool HasProfile { get; set; } = false;
    public StudentDetail StudentDetail {get; set;} = default!;
}
