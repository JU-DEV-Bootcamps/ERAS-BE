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


        public CreateComponentCommandHandler(IComponentRepository componentRepository, ILogger<CreateComponentCommandHandler> logger)
        {
            _componentRepository = componentRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<Component>> Handle(CreateComponentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Component? componentnDB = await _componentRepository.GetByNameAsync(request.Component.Name);
                if (componentnDB != null) return new CreateCommandResponse<Component>(componentnDB, 0, "Success", true);

                Component? component = request.Component?.ToDomain();
                if (component == null) return new CreateCommandResponse<Component>(null,0, "Error", false);
                Component createdComponent = await _componentRepository.AddAsync(component);

                return new CreateCommandResponse<Component>(createdComponent, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the component: ");
                return new CreateCommandResponse<Component>(null,0, "Error", false);
            }
        }
    }
}
