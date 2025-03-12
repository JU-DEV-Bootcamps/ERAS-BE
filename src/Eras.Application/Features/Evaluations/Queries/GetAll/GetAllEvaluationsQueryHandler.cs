using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Evaluations.Commands.CreateEvaluation;
using Eras.Application.Features.PollInstances.Queries.GetPollInstanceByLastDays;
using Eras.Application.Models;
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
    public class GetAllEvaluationsQueryHandler : IRequestHandler<GetAllEvaluationsQuery, List<Evaluation>>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ILogger<GetAllEvaluationsQueryHandler> _logger;

        public GetAllEvaluationsQueryHandler(IEvaluationRepository evaluationRepository, ILogger<GetAllEvaluationsQueryHandler> logger)
        {
            _evaluationRepository = evaluationRepository;
            _logger = logger;
        }
        public async Task<List<Evaluation>> Handle(GetAllEvaluationsQuery request, CancellationToken cancellationToken)
        {
            try
            {             
                return _evaluationRepository.GetAllAsync().Result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get information. Reason: {ResponseMessage}", ex.Message);
                return new List<Evaluation>();
            }
        }
    }
}
