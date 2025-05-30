using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays
{
    internal class GetPollInstancesByLastDaysQueryHandler : IRequestHandler<GetPollInstancesByLastDaysQuery, GetQueryResponse<List<PollInstance>>>
    {
        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly ILogger<GetPollInstancesByLastDaysQueryHandler> _logger;

        public GetPollInstancesByLastDaysQueryHandler(IPollInstanceRepository PollInstanceRepository, ILogger<GetPollInstancesByLastDaysQueryHandler> Logger)
        {
            _pollInstanceRepository = PollInstanceRepository;
            _logger = Logger;
        }

        public async Task<GetQueryResponse<List<PollInstance>>> Handle(GetPollInstancesByLastDaysQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                IEnumerable<PollInstance> pollInstances = await _pollInstanceRepository.GetByLastDays(Request.LastDays);
                return new GetQueryResponse<List<PollInstance>>(pollInstances.ToList(), "PollInstances obtained", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting the poll instance: ");
                return new GetQueryResponse<List<PollInstance>>([], "Error", false);
            }
        }
    }
}
