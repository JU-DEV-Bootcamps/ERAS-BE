using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays
{
    internal class GetPollInstancesByLastDaysQueryHandler: IRequestHandler<GetPollInstancesByLastDaysQuery, QueryManyResponse<PollInstance>>
    {
        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly ILogger<GetPollInstancesByLastDaysQueryHandler> _logger;

        public GetPollInstancesByLastDaysQueryHandler(IPollInstanceRepository pollInstanceRepository, ILogger<GetPollInstancesByLastDaysQueryHandler> logger)
        {
            _pollInstanceRepository = pollInstanceRepository;
            _logger = logger;
        }

        public async Task<QueryManyResponse<PollInstance>> Handle(GetPollInstancesByLastDaysQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<PollInstance> pollInstances = await _pollInstanceRepository.GetByLastDays(request.LastDays);
                return new QueryManyResponse<PollInstance>(pollInstances, "PollInstances obtained", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting the poll instance: ");
                return new QueryManyResponse<PollInstance>(null, "Error", false);
            }
        }
    }
}
