

using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class GetRemissionByIdQueryHandler
    : IRequestHandler<GetRemissionByIdQuery, AssessmentDto?>
{
    private readonly IAssessmentRepository _repository;
    private readonly IMapper<Assessment, AssessmentDto> _mapper;
    private readonly ILogger<GetRemissionByIdQueryHandler> _logger;
    private readonly IStudentRepository _studentRepository;

    public GetRemissionByIdQueryHandler(
        IAssessmentRepository repository,
        IMapper<Assessment, AssessmentDto> mapper,
        ILogger<GetRemissionByIdQueryHandler> logger,
        IStudentRepository studentRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _studentRepository = studentRepository;
        _studentRepository = studentRepository;
    }

    public async Task<AssessmentDto?> Handle(
        GetRemissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.Id);
        if (assessment is null) return null;

        var uniqueIds = assessment.StudentIds ?? Array.Empty<int>();
        
        var students = uniqueIds.Any()
            ? await _studentRepository.GetByIdsAsync(uniqueIds, cancellationToken)
            : Array.Empty<Student>();

        var studentDict = students.ToDictionary(s => s.Id);

        var dto = _mapper.Map(assessment);
        var studentDtos = assessment.StudentIds
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
    }
}