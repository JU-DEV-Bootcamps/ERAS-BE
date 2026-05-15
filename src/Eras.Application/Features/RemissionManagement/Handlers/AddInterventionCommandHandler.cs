using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class AddInterventionCommandHandler
    : IRequestHandler<AddInterventionCommand, InterventionDto>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<IndividualInterventionDto, IndividualIntervention> _individualMapper;
    private readonly IMapper<GroupInterventionDto, GroupIntervention> _groupMapper;

    public AddInterventionCommandHandler(
        IAssessmentRepository repository,
        IMapper<IndividualInterventionDto, IndividualIntervention> individualMapper,
        IMapper<GroupInterventionDto, GroupIntervention> groupMapper)
    {
        _repository = repository;
        _individualMapper = individualMapper;
        _groupMapper = groupMapper;
    }

    public async Task<InterventionDto> Handle(
        AddInterventionCommand request,
        CancellationToken cancellationToken)
    {
        Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.AssessmentId);

        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{request.AssessmentId}' not found.");

        Intervention newIntervention = MapIntervention(request.Intervention);

        Intervention persisted = await _repository.AddInterventionAsync(request.AssessmentId, newIntervention);

        return request.Intervention switch
        {
            IndividualInterventionDto => new IndividualInterventionDto
            {
                Id = persisted.Id,
                DateUtc = persisted.DateUtc,
                Activity = persisted.Activity,
                Area = persisted.Area,
                NumberOfGuests = persisted.NumberOfGuests,
                NumberOfParticipants = persisted.NumberOfParticipants,
                Professional = persisted.Professional,
                Comments = persisted.Comments,
                Attachments = persisted.Attachments
            },
            GroupInterventionDto => new GroupInterventionDto
            {
                Id = persisted.Id,
                DateUtc = persisted.DateUtc,
                Activity = persisted.Activity,
                Area = persisted.Area,
                NumberOfGuests = persisted.NumberOfGuests,
                NumberOfParticipants = persisted.NumberOfParticipants,
                Professional = persisted.Professional,
                Comments = persisted.Comments,
                Attachments = persisted.Attachments,
                Area = ((GroupIntervention)persisted).Area,
                ParticipantIds = ((GroupIntervention)persisted).ParticipantIds
            },
            _ => throw new NotSupportedException(
                $"Intervention DTO type '{request.Intervention.GetType().Name}' is not supported.")
        };
    }

    private Intervention MapIntervention(InterventionDto dto)
    {
        return dto switch
        {
            IndividualInterventionDto individual => _individualMapper.Map(individual),
            GroupInterventionDto group => _groupMapper.Map(group),
            _ => throw new NotSupportedException(
                $"Intervention DTO type '{dto.GetType().Name}' is not supported.")
        };
    }
}