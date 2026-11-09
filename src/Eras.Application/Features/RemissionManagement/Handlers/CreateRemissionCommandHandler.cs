

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities.AssessmentManagement;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class CreateRemissionCommandHandler
    : IRequestHandler<CreateRemissionCommand, AssessmentDto>
{
    private readonly IMapper<AssessmentDto, Assessment> _toDomainMapper;
    private readonly IMapper<Assessment, AssessmentDto> _toDtoMapper;
    private readonly IValidator<Assessment> _validator;
    private readonly IAssessmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IAssessmentActivityRepository _activityRepository;

    public CreateRemissionCommandHandler(
        IMapper<AssessmentDto, Assessment> toDomainMapper,
        IMapper<Assessment, AssessmentDto> toDtoMapper,
        IValidator<Assessment> validator,
        IAssessmentRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IAssessmentActivityRepository activityRepository)
    {
        _toDomainMapper = toDomainMapper;
        _toDtoMapper = toDtoMapper;
        _validator = validator;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _activityRepository = activityRepository;
    }

    public async Task<AssessmentDto> Handle(
        CreateRemissionCommand request,
        CancellationToken cancellationToken)
    {
        Assessment entity = _toDomainMapper.Map(request.Remission);

        await ValidationHelper.ValidateAndThrowAsync(_validator, entity, cancellationToken);

        // Saving the assessment and recording "Assessment created" are one unit of work.
        Assessment persisted = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            Assessment created = await _repository.AddAsync(entity);

            await _activityRepository.AddRangeAsync(
                [AssessmentActivityFactory.Created(created.Id, ActivityActor.From(_currentUser), DateTime.UtcNow)],
                cancellationToken);

            return created;
        });

        return _toDtoMapper.Map(persisted);
    }
}