using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities.AssessmentManagement;
using MediatR;
using Eras.Domain.Entities;

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

        var students = uniqueIds.Any()
            ? await _studentRepository.GetByIdsAsync(uniqueIds, cancellationToken)
            : Array.Empty<Student>();

        var studentDict = students.ToDictionary(s => s.Id);

        return entities.Select(entity =>
        {
            var dto = _mapper.Map(entity);
            var studentDtos = entity.StudentIds
                .Select(id =>
                {
                    if (studentDict.TryGetValue(id, out var student))
                    {
                        return new StudentProfileDto
                        {
                            Id = student.Id,
                            Name = student.Name,
                            Email = student.Email,
                        };
                    }

                    return new StudentProfileDto
                    {
                        Id = id,
                        Name = $"ID {id}",
                        Email = string.Empty,
                    };
                })
                .ToArray();
            return dto with
            {
                Students = studentDtos,
            };
        }).ToArray();
    }
}