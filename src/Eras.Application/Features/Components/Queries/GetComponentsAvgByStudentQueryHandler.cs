using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Components.Queries
{
    public class GetComponentsAvgByStudentQueryHandler : IRequestHandler<GetComponentsAvgByStudentQuery, List<ComponentsAvg>>
    {
        private readonly IComponentsAvgRepository _componentsAvgRepository;
        private readonly ILogger<GetComponentsAvgByStudentQueryHandler> _logger;

        public GetComponentsAvgByStudentQueryHandler(IComponentsAvgRepository componentsAvgRepository, ILogger<GetComponentsAvgByStudentQueryHandler> logger)
        {
            _componentsAvgRepository = componentsAvgRepository;
            _logger = logger;
        }

        public async Task<List<ComponentsAvg>> Handle(GetComponentsAvgByStudentQuery request, CancellationToken cancellationToken)
        {
            var listofComponents = await _componentsAvgRepository.ComponentsAvgByStudent(request.StudentId, request.PollId);
            return listofComponents;
        }
    }
}
