using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Components.Commands.CreateCommand
{
    public class CreateComponentCommandHandler : IRequestHandler<CreateComponentCommand, CreateCommandResponse<Component>>
    {
        private readonly IComponentRepository _componentRepository;
        private readonly ILogger<CreateComponentCommandHandler> _logger;


        public CreateComponentCommandHandler(IComponentRepository ComponentRepository, ILogger<CreateComponentCommandHandler> Logger)
        {
            _componentRepository = ComponentRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Component>> Handle(CreateComponentCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                if (Request.Component == null)
                {
                    _logger.LogError("Component is null");
                    return new CreateCommandResponse<Component>(null, 0, "Error", false);
                }
                Component? componentDB = await _componentRepository.GetByNameAsync(Request.Component.Name);
                if (componentDB != null) return new CreateCommandResponse<Component>(componentDB, 0, "Success", true);

                Component? component = Request.Component?.ToDomain();
                if (component == null) return new CreateCommandResponse<Component>(null, 0, "Error", false);
                Component createdComponent = await _componentRepository.AddAsync(component);

                return new CreateCommandResponse<Component>(createdComponent, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the component: ");
                return new CreateCommandResponse<Component>(null, 0, "Error", false);
            }
        }
    }
}
