using System.Windows.Input;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Validation;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement.StatusManagement;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class ReplaceInterventionCommandHandler
    : IRequestHandler<ReplaceInterventionCommand, UpdateInterventionDto>
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserIdentityProvider _userIdentityProvider;
    private readonly ILogger<ReplaceInterventionCommandHandler> _logger;
    private readonly IMapper<UpdateInterventionDto, Intervention> _mapper;
    private readonly IValidator<StatusTransitionRequest<InterventionStatus>> _interventionStatusValidator;

    public ReplaceInterventionCommandHandler(
        IAssessmentRepository AssessmentRepository,
        IAttachmentRepository AttachmentRepository,
        IAttachmentService AttachmentService,
        IUnitOfWork UnitOfWork,
        IUserIdentityProvider UserIdentityProvider,
        ILogger<ReplaceInterventionCommandHandler> Logger,
        IMapper<UpdateInterventionDto, Intervention> Mapper,
        IValidator<StatusTransitionRequest<InterventionStatus>> InterventionStatusValidator)
    {
        _assessmentRepository = AssessmentRepository;
        _attachmentRepository = AttachmentRepository;
        _attachmentService = AttachmentService;
        _unitOfWork = UnitOfWork;
        _userIdentityProvider = UserIdentityProvider;
        _logger = Logger;
        _mapper = Mapper;
        _interventionStatusValidator = InterventionStatusValidator;
    }

    public async Task<UpdateInterventionDto> Handle(ReplaceInterventionCommand Request, CancellationToken CancellationToken)
    {
        Assessment? assessment = await _assessmentRepository.GetByIdWithInterventionsAsync(Request.AssessmentId);
        if (assessment is null)
            throw new KeyNotFoundException($"Assessment '{Request.AssessmentId}' not found.");

        Intervention? oldIntervention = assessment.Interventions.FirstOrDefault(i => i.Id == Request.OldInterventionId);
        if (oldIntervention is null)
            throw new KeyNotFoundException($"Intervention '{Request.OldInterventionId}' not found.");

        try
        {
            await ValidationHelper.ValidateAndThrowAsync(_interventionStatusValidator,
                        new StatusTransitionRequest<InterventionStatus>(oldIntervention.Status, Request.NewIntervention.Status),
                        CancellationToken);
            int[] idsToRemove = Request.AttachmentIdsToRemove ?? Array.Empty<int>();
            List<string> storageKeysToDelete = new();

            if (idsToRemove.Length > 0)
            {
                IEnumerable<Attachment> attachmentsToRemove = await _attachmentRepository.GetByEntityAsync(
                    InterventionConstants.AttachmentEntityType, Request.OldInterventionId);

                HashSet<int> belongingIds = attachmentsToRemove.Select(a => a.Id).ToHashSet();
                int[] foreignIds = idsToRemove.Where(id => !belongingIds.Contains(id)).ToArray();
                if (foreignIds.Length > 0)
                    throw new BussinessException(
                        $"Attachment ID(s) [{string.Join(", ", foreignIds)}] do not belong to intervention.", 409);

                storageKeysToDelete = attachmentsToRemove
                    .Where(a => idsToRemove.Contains(a.Id))
                    .Select(a => a.StorageKey)
                    .ToList();
            }

            Intervention mapped = MapIntervention(Request.NewIntervention);
            mapped.Id = Request.OldInterventionId;

            Intervention persisted = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _assessmentRepository.DeleteInterventionAsync(Request.AssessmentId, Request.OldInterventionId);

                Intervention created = await _assessmentRepository.AddInterventionAsync(
                    Request.AssessmentId, mapped);
                if (idsToRemove.Length > 0)
                    await _attachmentRepository.DeleteByIdsAsync(idsToRemove);

                if (Request.DraftSessionId.HasValue)
                    await _attachmentService.ClaimDraftAttachmentsAsync(
                        Request.DraftSessionId.Value,
                        InterventionConstants.AttachmentEntityType,
                        created.Id, _userIdentityProvider.UserId,
                        CancellationToken);

                return created;
            });

            // Post-commit: physical file deletion (best-effort)
            foreach (string storageKey in storageKeysToDelete)
            {
                try
                {
                    await _attachmentService.DeleteByStorageKeyAsync(storageKey, CancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Physical deletion of storage key '{StorageKey}' failed after commit; file may be orphaned.",
                        storageKey);
                }
            }

            IEnumerable<Attachment> currentAttachments = await _attachmentRepository.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, persisted.Id);

            return new UpdateInterventionDto(
                 DateUtc: persisted.DateUtc,
                 Activity: persisted.Activity!,
                 Area: persisted.Area!,
                 NumberOfParticipants: persisted.NumberOfParticipants ?? 0,
                 Professional: persisted.Professional,
                 Comments: persisted.Comments,
                 StudentIds: persisted.StudentIds,
                 Attendance: persisted.Attendance,
                 Mode: persisted.Mode,
                 Kind: persisted.Kind,
                 Status: persisted.Status,
                 Remarks: persisted.Remarks,
                 RiskLevel: persisted.RiskLevel,
                 RiskLevelName: persisted.RiskLevelName,
                 EndRiskLevelName: persisted.EndRiskLevelName,
                 Id: persisted.Id
            );

        }
        catch (ValidationException Ex)
        {
            throw new OperationCanceledException($"Error updating assessment: Some status cannot be updated. ${Ex.Message}");
        }
    }

    public Intervention MapIntervention(UpdateInterventionDto intervention)
    {
        return intervention.Kind switch
        {
            InterventionKind.Individual => new IndividualIntervention
            {
                DateUtc = intervention.DateUtc,
                Activity = intervention.Activity,
                Area = intervention.Area,
                NumberOfParticipants = intervention.NumberOfParticipants,
                Professional = intervention.Professional,
                Comments = intervention.Comments,
                StudentIds = intervention.StudentIds,
                Attendance = intervention.Attendance,
                Mode = intervention.Mode,
                Status = intervention.Status,
                Remarks = intervention.Remarks,
                RiskLevelName = intervention.RiskLevelName,
                EndRiskLevelName = intervention.EndRiskLevelName,
            },
            InterventionKind.Group => new GroupIntervention
            {
                DateUtc = intervention.DateUtc,
                Activity = intervention.Activity,
                Area = intervention.Area,
                NumberOfParticipants = intervention.NumberOfParticipants,
                Professional = intervention.Professional,
                Comments = intervention.Comments,
                StudentIds = intervention.StudentIds,
                Attendance = intervention.Attendance,
                Mode = intervention.Mode,
                Status = intervention.Status,
                Remarks = intervention.Remarks,
                RiskLevelName = intervention.RiskLevelName,
                EndRiskLevelName = intervention.EndRiskLevelName,
            },
            _ => throw new NotSupportedException(
               $"Intervention type '{intervention.GetType().Name}' is not supported.")
        };
     }
}
