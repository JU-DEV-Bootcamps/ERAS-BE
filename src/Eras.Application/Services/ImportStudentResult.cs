namespace Eras.Application.Services
{
    /// <summary>Outcome of processing a single student's import (one ImportJobItem).</summary>
    public sealed record ImportStudentResult(bool Success, string? ErrorMessage);
}
