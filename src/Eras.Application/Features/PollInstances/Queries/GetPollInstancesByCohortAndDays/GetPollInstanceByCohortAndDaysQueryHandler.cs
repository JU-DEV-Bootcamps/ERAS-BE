using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays
{
    public class GetPollInstanceByCohortAndDaysQueryHandler :
        IRequestHandler<GetPollInstanceByCohortAndDaysQuery, GetQueryResponse<IEnumerable<PollInstanceDTO>>>
    {

        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly ILogger<GetPollInstanceByCohortAndDaysQueryHandler> _logger;

        public GetPollInstanceByCohortAndDaysQueryHandler(IPollInstanceRepository PollInstanceRepository, ILogger<GetPollInstanceByCohortAndDaysQueryHandler> Logger)
        {
            _pollInstanceRepository = PollInstanceRepository;
            _logger = Logger;
        }
        public async Task<GetQueryResponse<IEnumerable<PollInstanceDTO>>> Handle(GetPollInstanceByCohortAndDaysQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                var pollInstances = await _pollInstanceRepository.GetByCohortIdAndLastDays(Request.CohortId, Request.Days, Request.LastVersion, Request.PollUuid);
                var pollInstanceDTOs = pollInstances.Select(PollInstance => PollInstanceMapper.ToDTO(PollInstance)).OrderByDescending(Pi => Pi.FinishedAt);
                return new GetQueryResponse<IEnumerable<PollInstanceDTO>>(pollInstanceDTOs ,"Success", true );
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting the poll instance");
                return new GetQueryResponse<IEnumerable<PollInstanceDTO>>([], "Failed", false);
            }
        }
    }
}
