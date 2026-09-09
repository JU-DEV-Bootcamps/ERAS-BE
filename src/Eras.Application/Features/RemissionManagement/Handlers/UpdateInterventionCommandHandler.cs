using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Error.Bussiness;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;
using Eras.Application.Mappers.AssessmentManagement;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpdateInterventionCommandHandler : IRequestHandler<UpdateInterventionCommand, InterventionDto>
{
    private readonly IAttachmentService _attachmentService;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserIdentityProvider _userIdentityProvider;
    private readonly ILogger<UpdateInterventionCommandHandler> _logger;
    private readonly IMapper<UpdateInterventionDto, Intervention> _mapper;

    public UpdateInterventionCommandHandler(
        IAttachmentService Service,
        IAttachmentRepository AttachmentRepository,
        IAssessmentRepository AssessmentRepository,
        IUnitOfWork UnitOfWork,
        IUserIdentityProvider Provider,
        ILogger<UpdateInterventionCommandHandler> Logger,
        IMapper<UpdateInterventionDto, Intervention> Mapper)
    {
        _attachmentService = Service;
        _attachmentRepository = AttachmentRepository;
        _assessmentRepository = AssessmentRepository;
        _unitOfWork = UnitOfWork;
        _userIdentityProvider = Provider;
        _logger = Logger;
        _mapper = Mapper;
    }

    public async Task<InterventionDto> Handle(UpdateInterventionCommand Request, CancellationToken CancellationToken)
    {
        Assessment? assessment = await _assessmentRepository.GetByIdWithInterventionsAsync(Request.AssessmentId);
        if (assessment is null)
        {
            throw new KeyNotFoundException($"Assessment '{Request.AssessmentId}' not found.");
        }

        Intervention? existingIntervention = assessment.Interventions.FirstOrDefault(I => I.Id == Request.InterventionId);
        if (existingIntervention is null)
        {
            throw new KeyNotFoundException($"Intervention '{Request.InterventionId}' not found in assessment '{Request.AssessmentId}'.");
        }
        int[] idsToRemove = Request.AttachmentIdsToRemove ?? Array.Empty<int>();
        List<string> storageKeysToDelete = new();

        int? draftSessionId = Request.DraftSessionId;//hi
        List<Attachment> attachmentsToDelete = new();//hi?

        //Intervention updated = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        //{
        //    Intervention mapped = _mapper.Map(Request.Intervention);
        //    mapped.Id = Request.InterventionId;
        //    Intervention persisted = await _assessmentRepository.UpdateAsync(assessment);

        if (idsToRemove.Length > 0)
        {
            IEnumerable<Attachment> attachmentsToRemove = await _attachmentRepository.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, Request.InterventionId);

            HashSet<int> belongingIds = attachmentsToRemove.Select(a => a.Id).ToHashSet();
            int[] foreignIds = idsToRemove.Where(Id => !belongingIds.Contains(Id)).ToArray();
            if (foreignIds.Length > 0) {
                throw new BussinessException(
                        $"Attachment ID(s) [{string.Join(", ", foreignIds)}] do not belong to " +
                        $"intervention '{Request.InterventionId}' or were already removed.", 409);
            }
            storageKeysToDelete = attachmentsToRemove
               .Where(a => idsToRemove.Contains(a.Id))
               .Select(a => a.StorageKey)
               .ToList();
        }

        Intervention mapped = _mapper.Map(Request.Intervention);
        mapped.Id = Request.InterventionId;

        Intervention persisted = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            Intervention updated = await _assessmentRepository.UpdateInterventionAsync(Request.AssessmentId, mapped);
            if (idsToRemove.Length > 0)
                await _attachmentRepository.DeleteByIdsAsync(idsToRemove);

            if (Request.DraftSessionId.HasValue)
            {
                await _attachmentService.ClaimDraftAttachmentsAsync(
                    Request.DraftSessionId.Value,
                    InterventionConstants.AttachmentEntityType,
                    Request.InterventionId,
                    _userIdentityProvider.UserId,
                    CancellationToken);
            }
            return updated;
        });
        //int deleted = await _attachmentRepository.DeleteByIdsAndEntityAsync(
        //    idsToRemove, InterventionConstants.AttachmentEntityType, Request.InterventionId);

        //if (deleted != idsToRemove.Length)
        //{
        //    throw new BussinessException($"One or more attachment IDs don't belong to this intervention or were already removed.", 409);
        //}
    //}
//        if (draftSessionId.HasValue)
//        {
//            await _attachmentService.ClaimDraftAttachmentsAsync(
//                draftSessionId.Value,
//                InterventionConstants.AttachmentEntityType,
//                Request.InterventionId,
//                _userIdentityProvider.UserId,
//                CancellationToken);
//}
        //    return persisted;
        //});


        // physical file deletion
        foreach (string storageKey in storageKeysToDelete)
        {
            try
            {
                await _attachmentService.DeleteByStorageKeyAsync(storageKey, CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to physically delete attachment {AttachmentId} after row removal; file may be orphaned.", attachment.Id);
            }
        }

        return Request.Intervention.Kind switch
        {
            InterventionKind.Individual => new IndividualInterventionDto
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
            InterventionKind.Group => new GroupInterventionDto
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

    //private Intervention MapIntervention(InterventionDto Dto)
    //{
    //    return Dto switch
    //    {
    //        IndividualInterventionDto individual => _individualMapper.Map(individual),
    //        GroupInterventionDto group => _groupMapper.Map(group),
    //        _ => throw new NotSupportedException(
    //            $"Intervention DTO type '{Dto.GetType().Name}' is not supported.")
    //    };
    //}
}