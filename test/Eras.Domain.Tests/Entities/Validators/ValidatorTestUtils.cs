using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Domain.Tests.Entities.Validators;
public sealed class ValidatableIntervention
{
    public DateTime DateUtc { get; set; } = DateTime.Now;
    public string? Activity { get; set; } = "Workshop";
    public string? Area { get; set; } = "Psychology";
    public string? Professional { get; set; } = "Pedro Cardona";
    public string? Comments { get; set; } = "First session for student.";
    public IReadOnlyCollection<int> StudentIds { get; set; } = [1];
    public string? Remarks { get; set; } = "First session.";
    public IReadOnlyCollection<string> Attachments { get; set; } = ["file.pdf"];
    public double? RiskLevel { get; set; } = 3.0;
}

public sealed class ValidatableAssessment
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = "System";
    public string Service { get; set; } = "Academic Support";
    public string? AssignedProfessional { get; set; }
    public int[] StudentIds { get; set; } = [1, 4, 23];
    public string? Diagnosis { get; set; }
    public string? Objective { get; set; }
    public string? Comments { get; set; }
    public InterventionPlan? Plan { get; set; }
    public IReadOnlyCollection<Intervention> Interventions { get; set; } = [];
}

public sealed class ValidatableInterventionPlan
{
    public int? SessionsPerWeek { get; set; }
    public string? ScheduleNotes { get; set; }
}