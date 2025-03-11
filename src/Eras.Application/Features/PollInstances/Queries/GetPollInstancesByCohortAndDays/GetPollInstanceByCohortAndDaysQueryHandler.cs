using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays;
using Eras.Application.Mappers;
using Eras.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays
{
    public class GetPollInstanceByCohortAndDaysQueryHandler :
        IRequestHandler<GetPollInstanceByCohortAndDaysQuery, GetQueryResponse<IEnumerable<PollInstanceDTO>>>
    {

        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly ILogger<GetPollInstanceByCohortAndDaysQueryHandler> _logger;

        public GetPollInstanceByCohortAndDaysQueryHandler(IPollInstanceRepository pollInstanceRepository, ILogger<GetPollInstanceByCohortAndDaysQueryHandler> logger)
        {
            _pollInstanceRepository = pollInstanceRepository;
            _logger = logger;
        }
        public async Task<GetQueryResponse<IEnumerable<PollInstanceDTO>>> Handle(GetPollInstanceByCohortAndDaysQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pollInstances = await _pollInstanceRepository.GetByCohortIdAndLastDays(request.CohortId, request.Days);
                var pollInstanceDTOs = pollInstances.Select(pollInstance => PollInstanceMapper.ToDTO(pollInstance)).OrderByDescending(pi => pi.FinishedAt);
                return new GetQueryResponse<IEnumerable<PollInstanceDTO>>(pollInstanceDTOs ,"Success", true );
            }
            catch(Exception ex)
            {
                return new GetQueryResponse<IEnumerable<PollInstanceDTO>>([], "Failed", false);
            }
        }
    }
}
