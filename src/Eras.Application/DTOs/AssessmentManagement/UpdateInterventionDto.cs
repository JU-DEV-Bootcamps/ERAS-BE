
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record UpdateInterventionDto(
    DateTime DateUtc,
    string Activity,
    string Area,
    int NumberOfParticipants,
    string? Professional,
    string? Comments,
    IReadOnlyCollection<int> StudentIds,
    IReadOnlyDictionary<int, bool> Attendance,
    InterventionMode Mode,
    InterventionKind Kind,
    InterventionStatus Status,
    string? Remarks,
    IReadOnlyCollection<AttachmentDto> Attachments,
    double? RiskLevel,
    InterventionLevel RiskLevelName,
    InterventionLevel? EndRiskLevelName,
    int? Id
);
