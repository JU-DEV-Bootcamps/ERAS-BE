namespace Eras.Application.Models.Response.Controllers.StudentsController;
public class GetAllStudentsQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsImported { get; set; } = false;
}
