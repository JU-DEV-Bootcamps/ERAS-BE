using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
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

    public GetRemissionByIdQueryHandler(
        IAssessmentRepository repository,
        IMapper<Assessment, AssessmentDto> mapper,
        ILogger<GetRemissionByIdQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AssessmentDto?> Handle(
        GetRemissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            Assessment? assessment = await _repository.GetByIdWithInterventionsAsync(request.Id);

            return assessment is null ? null : _mapper.Map(assessment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting assessment by id: {Message}", ex.Message);
            return null;
        }
    }
}