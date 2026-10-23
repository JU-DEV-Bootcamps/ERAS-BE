using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.UsersManagement;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;

public sealed class GetMyProfileQueryHandler(
    IErasUsersRepository Repository,
    IAssessmentRepository AssessmentRepository)
    : IRequestHandler<GetMyProfileQuery, MyProfileDTO>
{
    private readonly IErasUsersRepository _repository = Repository;
    private readonly IAssessmentRepository _assessmentRepository = AssessmentRepository;

    public async Task<MyProfileDTO> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        ErasUserDTO? profile = request.Sub != null
            ? await _repository.GetErasUserBySubAsync(request.Sub) ?? await _repository.GetErasUserByEmailAsync(request.Email)
            : await _repository.GetErasUserByEmailAsync(request.Email);

        if (profile is null)
            throw new NotFoundException($"User {request.Email} not found.");

        string? sub = profile.Sub ?? request.Sub;
        int activeAssessments = 0;
        int activeInterventions = 0;
        if (!string.IsNullOrWhiteSpace(sub))
        {
            activeAssessments = await _assessmentRepository.CountActiveAssessmentsForUserAsync(sub);
            activeInterventions = await _assessmentRepository.CountActiveInterventionsForUserAsync(sub);
        }

        return MyProfileDTO.From(profile, activeAssessments, activeInterventions);
    }
}
