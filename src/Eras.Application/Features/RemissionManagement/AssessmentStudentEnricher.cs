using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Features.RemissionManagement;

/// <summary>
/// Shared by every query handler that returns <see cref="AssessmentDto"/> collections: resolves
/// each assessment's <c>StudentIds</c> into full <see cref="StudentProfileDto"/> entries (name,
/// email, average risk), since <see cref="Assessment"/> only stores student ids, not a navigable
/// relationship the EF mapper can follow.
/// </summary>
public static class AssessmentStudentEnricher
{
    public static async Task<IReadOnlyCollection<AssessmentDto>> EnrichWithStudentsAsync(
        IEnumerable<Assessment> entities,
        IMapper<Assessment, AssessmentDto> mapper,
        IStudentRepository studentRepository,
        CancellationToken cancellationToken = default)
    {
        List<Assessment> entityList = entities.ToList();

        List<int> uniqueIds = entityList
            .SelectMany(e => e.StudentIds ?? Array.Empty<int>())
            .Distinct()
            .ToList();

        IEnumerable<Student> students = uniqueIds.Count > 0
            ? await studentRepository.GetByIdsAsync(uniqueIds, cancellationToken)
            : Array.Empty<Student>();

        Dictionary<int, double> avgRisks = uniqueIds.Count > 0
            ? await studentRepository.GetAverageRiskByStudentIdsAsync(uniqueIds)
            : new Dictionary<int, double>();

        Dictionary<int, Student> studentDict = students.ToDictionary(s => s.Id);

        return entityList.Select(entity =>
        {
            AssessmentDto dto = mapper.Map(entity);
            StudentProfileDto[] studentDtos = entity.StudentIds
                .Select(id =>
                {
                    if (studentDict.TryGetValue(id, out Student? student))
                    {
                        double avgRisk = avgRisks.TryGetValue(id, out double risk) ? risk : 0;
                        return new StudentProfileDto
                        {
                            Id = student.Id,
                            Name = student.Name,
                            Email = student.Email,
                            AvgRiskLevel = avgRisk,
                        };
                    }

                    return new StudentProfileDto
                    {
                        Id = id,
                        Name = $"ID {id}",
                        Email = string.Empty,
                        AvgRiskLevel = 0,
                    };
                })
                .ToArray();

            return dto with { Students = studentDtos };
        }).ToArray();
    }
}
