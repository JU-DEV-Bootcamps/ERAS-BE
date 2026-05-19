using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities.AssessmentManagement;
using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetAllRemissionsQueryHandler
    : IRequestHandler<GetAllRemissionsQuery, IReadOnlyCollection<AssessmentDto>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper;
    private readonly IStudentRepository _studentRepository;

    public GetAllRemissionsQueryHandler(
        IAssessmentRepository repository,
        IMapper<Assessment, AssessmentDto> mapper,
        IStudentRepository studentRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _studentRepository = studentRepository;
    }

    public async Task<IReadOnlyCollection<AssessmentDto>> Handle(
        GetAllRemissionsQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Assessment> entities = await _repository.GetAllAsync();

        var uniqueIds = entities
            .SelectMany(e => e.StudentIds ?? Array.Empty<int>())
            .Distinct()
            .ToList();

        if (!uniqueIds.Any())
            return entities
                .Select(e => _mapper.Map(e) with { StudentNames = Array.Empty<string>() })
                .ToArray();

        var students = await _studentRepository.GetByIdsAsync(uniqueIds, cancellationToken);
        var nameDict = students.ToDictionary(s => s.Id, s => s.Name);

        return entities.Select(entity =>
        {
            var dto = _mapper.Map(entity);
            return dto with
            {
                StudentNames = entity.StudentIds
                    .Select(id => nameDict.TryGetValue(id, out var name) ? name : $"ID {id}")
                    .ToArray()
            };
        }).ToArray();
    }
}