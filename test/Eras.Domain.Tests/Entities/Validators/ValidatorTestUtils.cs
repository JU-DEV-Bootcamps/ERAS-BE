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