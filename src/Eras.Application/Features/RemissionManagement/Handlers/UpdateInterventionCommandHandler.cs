using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

using static Eras.Application.Models.Enums.RiskLevelEnum;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UpdateInterventionCommandHandler : IRequestHandler<UpdateInterventionCommand, UpdateInterventionDto>
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

    public async Task<UpdateInterventionDto> Handle(UpdateInterventionCommand Request, CancellationToken CancellationToken)
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

        if (idsToRemove.Length > 0)
        {
            IEnumerable<Attachment> attachmentsToRemove = await _attachmentRepository.GetByEntityAsync(
                InterventionConstants.AttachmentEntityType, Request.InterventionId);

            HashSet<int> belongingIds = attachmentsToRemove.Select(A => A.Id).ToHashSet();
            int[] foreignIds = idsToRemove.Where(Id => !belongingIds.Contains(Id)).ToArray();
            if (foreignIds.Length > 0) {
                throw new BussinessException(
                        $"Attachment ID(s) [{string.Join(", ", foreignIds)}] do not belong to " +
                        $"intervention '{Request.InterventionId}' or were already removed.", 409);
            }
            storageKeysToDelete = attachmentsToRemove
               .Where(A => idsToRemove.Contains(A.Id))
               .Select(A => A.StorageKey)
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
        
        // physical file deletion
        foreach (string storageKey in storageKeysToDelete)
        {
            try
            {
                await _attachmentService.DeleteByStorageKeyAsync(storageKey, CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to physically delete attachment after row removal; file may be orphaned.");
            }
        }

        IEnumerable<Attachment> currentAttachments = await _attachmentRepository.GetByEntityAsync(
            InterventionConstants.AttachmentEntityType, Request.InterventionId);

        IReadOnlyCollection<AttachmentDto> attachmentDtos = currentAttachments
            .Select(A => new AttachmentDto
            {
                Id = A.Id,
                EntityType = A.EntityType,
                EntityId = A.EntityId,
                OriginalFileName = A.OriginalFileName,
                MimeType = A.MimeType,
                SizeBytes = A.SizeBytes,
                ContentHash = A.ContentHash,
                CreatedAt = A.CreatedAt,
                CreatedBy = A.CreatedBy
            })
            .ToList();

        return new UpdateInterventionDto (
             DateUtc: persisted.DateUtc,
             Activity: persisted.Activity!,
             Area : persisted.Area!,
             NumberOfParticipants : persisted.NumberOfParticipants ?? 0,
             Professional : persisted.Professional,
             Comments : persisted.Comments,
             StudentIds : persisted.StudentIds,
             Attendance : persisted.Attendance,
             Mode : persisted.Mode,
             Kind : persisted.Kind,
             Status : persisted.Status,
             Remarks : persisted.Remarks,
             Attachments : attachmentDtos,
             RiskLevel : persisted.RiskLevel,
             RiskLevelName : persisted.RiskLevelName,
             EndRiskLevelName : persisted.EndRiskLevelName,
             Id : persisted.Id
        );
    }
}
