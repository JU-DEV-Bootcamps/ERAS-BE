using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays;
using Eras.Application.Models;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Evaluations.Queries.GetAll
{
    public class GetAllEvaluationsQueryHandler : IRequestHandler<GetAllEvaluationsQuery, PagedResult<Evaluation>>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ILogger<GetAllEvaluationsQueryHandler> _logger;

        public GetAllEvaluationsQueryHandler(IEvaluationRepository evaluationRepository, ILogger<GetAllEvaluationsQueryHandler> logger)
        {
            _evaluationRepository = evaluationRepository;
            _logger = logger;
        }
        public async Task<PagedResult<Evaluation>> Handle(GetAllEvaluationsQuery request, CancellationToken cancellationToken)
        {
            try
            {             
                var evaluations = await _evaluationRepository.GetPagedAsync(
                    request.Query.Page,
                    request.Query.PageSize
                );
                var totalCount = await _evaluationRepository.CountAsync();

                PagedResult<Evaluation> pagedResult = new PagedResult<Evaluation>(
                    totalCount,
                    evaluations.ToList()
                );

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get information. Reason: {ResponseMessage}", ex.Message);
                return new PagedResult<Evaluation>(
                    0,
                    new List<Evaluation>()
                );
            }
        }
    }
}
