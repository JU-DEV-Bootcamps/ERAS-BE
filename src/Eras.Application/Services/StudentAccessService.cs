using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

namespace Eras.Application.Services;

public sealed class StudentAccessService(
    ICurrentUserService CurrentUserService,
    KeycloakRoleNames RoleNames,
    IAssessmentRepository AssessmentRepository) : IStudentAccessService
{
    private const string ForbiddenMessage = "You do not have access to this student.";

    private string Role => ErasRole.Resolve(CurrentUserService.Roles, RoleNames);

    public bool IsAdministrator => Role == ErasRole.Administrator.Label;

    public async Task<IReadOnlyCollection<int>?> GetVisibleStudentIdsAsync(CancellationToken CancellationToken = default)
    {
        string role = Role;
        if (role == ErasRole.Administrator.Label || role == ErasRole.Officer.Label)
            return null;

        if (role == ErasRole.Professional.Label && !string.IsNullOrWhiteSpace(CurrentUserService.Sub))
            return await AssessmentRepository.GetStudentIdsByAssignedProfessionalAsync(CurrentUserService.Sub);

        throw new BussinessException(ForbiddenMessage, 403);
    }

    public async Task EnsureCanAccessStudentAsync(int StudentId, CancellationToken CancellationToken = default)
    {
        IReadOnlyCollection<int>? visibleIds = await GetVisibleStudentIdsAsync(CancellationToken);
        if (visibleIds is not null && !visibleIds.Contains(StudentId))
            throw new BussinessException(ForbiddenMessage, 403);
    }
}
