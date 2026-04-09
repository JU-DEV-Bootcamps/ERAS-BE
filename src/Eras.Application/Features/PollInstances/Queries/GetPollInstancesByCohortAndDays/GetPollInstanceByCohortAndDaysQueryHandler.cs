using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays
{
    public class GetPollInstanceByCohortAndDaysQueryHandler :
        IRequestHandler<GetPollInstanceByCohortAndDaysQuery, GetQueryResponse<PagedResult<PollInstanceDTO>>>
    {

        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly IEvaluationRepository _evaluationRepository; 
        private readonly ILogger<GetPollInstanceByCohortAndDaysQueryHandler> _logger;

        public GetPollInstanceByCohortAndDaysQueryHandler(IPollInstanceRepository PollInstanceRepository, IEvaluationRepository EvaluationRepository, ILogger<GetPollInstanceByCohortAndDaysQueryHandler> Logger)
        {
            _pollInstanceRepository = PollInstanceRepository;
            _evaluationRepository = EvaluationRepository;
            _logger = Logger;
        }
        public async Task<GetQueryResponse<PagedResult<PollInstanceDTO>>> Handle(GetPollInstanceByCohortAndDaysQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                DateTime? startDate = null;  
                DateTime? endDate = null;

            if (Request.EvaluationId.HasValue)
            {
                var evaluation = await _evaluationRepository.GetByIdAsync(Request.EvaluationId.Value);
                startDate = DateTime.SpecifyKind(evaluation.StartDate, DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(evaluation.EndDate, DateTimeKind.Utc)
                  .Date
                  .AddDays(1)
                  .AddTicks(-1);
            }

                var pollInstances = await _pollInstanceRepository.GetByCohortIdAndLastDays(Request.Pagination.Page, Request.Pagination.PageSize, Request.CohortId, Request.Days, Request.LastVersion, Request.PollUuid, startDate, endDate);
                var pollInstanceDTOs =
                    pollInstances.Items
                    .Select(PollInstance => PollInstanceMapper.ToDTO(PollInstance)).ToList();
                return new GetQueryResponse<PagedResult<PollInstanceDTO>>(new PagedResult<PollInstanceDTO>(pollInstances.Count, pollInstanceDTOs), "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting the poll instance");
                return new GetQueryResponse<PagedResult<PollInstanceDTO>>(new PagedResult<PollInstanceDTO>(0, []), "Failed", false);
            }
        }

    }
}
