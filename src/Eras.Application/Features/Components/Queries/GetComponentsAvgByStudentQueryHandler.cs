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

        public GetComponentsAvgByStudentQueryHandler(IComponentsAvgRepository ComponentsAvgRepository, ILogger<GetComponentsAvgByStudentQueryHandler> Logger)
        {
            _componentsAvgRepository = ComponentsAvgRepository;
            _logger = Logger;
        }

        public async Task<List<ComponentsAvg>> Handle(GetComponentsAvgByStudentQuery Request, CancellationToken CancellationToken)
        {
            var listofComponents = await _componentsAvgRepository.ComponentsAvgByStudent(Request.StudentId, Request.PollId);
            return listofComponents;
        }
    }
}
