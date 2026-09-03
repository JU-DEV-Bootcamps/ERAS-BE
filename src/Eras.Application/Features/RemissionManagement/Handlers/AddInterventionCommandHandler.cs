using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
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
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserIdentityProvider _userIdentityProvider;

    public AddInterventionCommandHandler(
        IAssessmentRepository Repository,
        IMapper<IndividualInterventionDto, IndividualIntervention> IndividualMapper,
        IMapper<GroupInterventionDto, GroupIntervention> GroupMapper,
        IAttachmentService AttachmentService,
        IUnitOfWork UnitOfWork,
        IUserIdentityProvider UserIdentityProvider)
    {
        _repository = Repository;
        _individualMapper = IndividualMapper;
        _groupMapper = GroupMapper;
        _attachmentService = AttachmentService;
        _unitOfWork = UnitOfWork;
        _userIdentityProvider = UserIdentityProvider;
    }

    public async Task<InterventionDto> Handle(
        AddInterventionCommand Request,
        CancellationToken CancellationToken)
    {
        Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(Request.AssessmentId);

        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{Request.AssessmentId}' not found.");

        Intervention newIntervention = MapIntervention(Request.Intervention);

        // Creating the intervention and claiming its drafted attachments (when requested) are one
        // unit of work: either both persist or neither does.
        Intervention persisted = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            Intervention created = await _repository.AddInterventionAsync(Request.AssessmentId, newIntervention);

            if (Request.DraftSessionId.HasValue)
            {
                await _attachmentService.ClaimDraftAttachmentsAsync(
                    Request.DraftSessionId.Value,
                    InterventionConstants.AttachmentEntityType,
                    created.Id,
                    _userIdentityProvider.UserId,
                    CancellationToken);
            }

            return created;
        });

        return Request.Intervention switch
        {
            IndividualInterventionDto => new IndividualInterventionDto
            {
                Id = persisted.Id,
                DateUtc = persisted.DateUtc,
                Activity = persisted.Activity,
                Area = persisted.Area,
                NumberOfParticipants = persisted.NumberOfParticipants,
                Professional = persisted.Professional,
                Comments = persisted.Comments,
                StudentIds = persisted.StudentIds,
                Attendance = persisted.Attendance,
                Mode = persisted.Mode,
                Status = persisted.Status,
                Remarks = persisted.Remarks,
                Attachments = persisted.Attachments,
                RiskLevel = persisted.RiskLevel,
                RiskLevelName = persisted.RiskLevelName
            },
            GroupInterventionDto => new GroupInterventionDto
            {
                Id = persisted.Id,
                DateUtc = persisted.DateUtc,
                Activity = persisted.Activity,
                Area = persisted.Area,
                NumberOfParticipants = persisted.NumberOfParticipants,
                Professional = persisted.Professional,
                Comments = persisted.Comments,
                StudentIds = persisted.StudentIds,
                Attendance = persisted.Attendance,
                Mode = persisted.Mode,
                Status = persisted.Status,
                Remarks = persisted.Remarks,
                Attachments = persisted.Attachments,
                RiskLevel = persisted.RiskLevel,
                RiskLevelName = persisted.RiskLevelName
            },
            _ => throw new NotSupportedException(
                $"Intervention DTO type '{Request.Intervention.GetType().Name}' is not supported.")
        };
    }

    private Intervention MapIntervention(InterventionDto Dto)
    {
        return Dto switch
        {
            IndividualInterventionDto individual => _individualMapper.Map(individual),
            GroupInterventionDto group => _groupMapper.Map(group),
            _ => throw new NotSupportedException(
                $"Intervention DTO type '{Dto.GetType().Name}' is not supported.")
        };
    }
}