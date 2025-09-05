
using Eras.Application.Mappers;
using Eras.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Entities;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Enums;

namespace Eras.Application.Features.JUServices.Commands.CreateJUService
{
    public class CreateJUServiceCommandHandler : IRequestHandler<CreateJUServiceCommand, CreateCommandResponse<JUService>>
    {
        private readonly IJUServiceRepository _juServiceRepository;
        private readonly ILogger<CreateJUServiceCommandHandler> _logger;

        public CreateJUServiceCommandHandler(IJUServiceRepository JUServiceRepository, ILogger<CreateJUServiceCommandHandler> Logger)
        {
            _juServiceRepository = JUServiceRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<JUService>> Handle(CreateJUServiceCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                int? Id = Request.Service.Id;

                if (Id != null)
                {
                    JUService? entityInDB = await _juServiceRepository.GetByIdAsync((int) Request.Service.Id);
                    if (entityInDB != null) return new CreateCommandResponse<JUService>(new JUService(), "Entity already exists", false,
                        CommandEnums.CommandResultStatus.AlreadyExists);
                }
                
                JUService juService = Request.Service.ToDomain();
                JUService response = await _juServiceRepository.AddAsync(juService);
                return new CreateCommandResponse<JUService>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the juService: " + Request.Service.Id);
                return new CreateCommandResponse<JUService>(new JUService(), "Error", false, CommandEnums.CommandResultStatus.Error);
            }
        }
    }
}
