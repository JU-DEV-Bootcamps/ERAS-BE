using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Commands.CreateVariable
{
    public class CreateVariableCommandHandler : IRequestHandler<CreateVariableCommand, CreateCommandResponse<Variable>>
    {
        private readonly IVariableRepository _variableRepository;
        private readonly ILogger<CreateVariableCommandHandler> _logger;

        public CreateVariableCommandHandler(
            IVariableRepository VariableRepository,
            ILogger<CreateVariableCommandHandler> Logger)
        {

            _variableRepository = VariableRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Variable>> Handle(CreateVariableCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                Variable? variableDB = await _variableRepository.GetByNameAsync(Request.Variable.Name);
                if (variableDB != null) return new CreateCommandResponse<Variable>(variableDB, 0, "Success", true);


                Variable? variable = Request.Variable?.ToDomain();
                if (variable!= null)
                {
                    variable.IdComponent = Request.ComponentId;
                    variable.IdPoll = Request.PollId;
                    Variable createdVariable = await _variableRepository.AddAsync(variable);
                    return new CreateCommandResponse<Variable>(createdVariable, 1, "Success", true);
                }
                return new CreateCommandResponse<Variable>(null, 0, "Error", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the variable: ");
                return new CreateCommandResponse<Variable>(null, 0, "Error", false);
            }
        }
    }
}
