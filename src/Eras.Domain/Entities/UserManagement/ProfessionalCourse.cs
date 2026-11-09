using Eras.Domain.Common;

namespace Eras.Domain.Entities.UserManagement;

public sealed class ProfessionalCourse : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
