using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent
{
    public class GetHigherRiskStudentByPollQueryHandler(
     ILogger<GetHigherRiskStudentByVariableQueryHandler> logger,
     IPollVariableRepository pollVariableRepository
   ) : IRequestHandler<GetHigherRiskStudentByPollQuery, GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>>
    {
        private readonly ILogger<GetHigherRiskStudentByVariableQueryHandler> _logger = logger;
        private readonly IPollVariableRepository _pollVariableRepository = pollVariableRepository;
        public int DefaultTakeNumber = 9999;

        public async Task<GetQueryResponse<List<(
            Answer answer,
            Variable variable,
            Student student)>>> Handle(
            GetHigherRiskStudentByPollQuery request,
            CancellationToken cancellationToken)
        {
            int TakeNStudents = request.Take.HasValue && request.Take.Value > 0 ? request.Take.Value : DefaultTakeNumber;
            try
            {
                var results = await _pollVariableRepository.GetByPollUuidAsync(request.PollInstanceUuid, request.VariableIds);
                var orderedStudents = results.OrderByDescending(s => s.Answer.RiskLevel).Take(TakeNStudents).ToList();
                return new GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>(orderedStudents, "Success", true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while calculating higher risk students: " + request);
                return new GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>(
                    new List<(Answer answer, Variable variable, Student student)>(),
                    $"Failed to retrieve top risk students by variable. Error {e.Message}",
                    false
                );
            }
        }
    }
}
