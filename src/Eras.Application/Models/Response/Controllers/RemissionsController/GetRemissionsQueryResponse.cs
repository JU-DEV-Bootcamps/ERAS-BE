
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Models.Response.Controllers.RemissionsController;

public class GetRemissionsQueryResponse
{
    // Esto debería ser una lista, no?
    public int Id { get; set; }
    public required Student Student { get; set; }
    public string Diagnostic { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public IEnumerable<JURemission> RemissionsList { get; set; } = [];
    public required AuditInfo Audit { get; set; }
}