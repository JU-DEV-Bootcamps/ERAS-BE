using Eras.Application.Mappers;
using Eras.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Entities;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Enums;

namespace Eras.Application.Features.Remmisions.Commands.CreateRemission
{
    public class CreateRemissionCommandHandler : IRequestHandler<CreateRemissionCommand, CreateCommandResponse<JURemission>>
    {
        private readonly IRemissionRepository _remissionRepository;
        private readonly ILogger<CreateRemissionCommandHandler> _logger;

        public CreateRemissionCommandHandler(IRemissionRepository RemissionRepository, ILogger<CreateRemissionCommandHandler> Logger)
        {
            _remissionRepository = RemissionRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<JURemission>> Handle(CreateRemissionCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                JURemission? entityInDB = await _remissionRepository.GetByIdAsync(Request.Remission.Id);
                if (entityInDB != null) return new CreateCommandResponse<JURemission>(new JURemission(), "Entity already exists", false,
                    CommandEnums.CommandResultStatus.AlreadyExists);

                JURemission remission = Request.Remission.ToDomain();
                JURemission response = await _remissionRepository.AddAsync(remission);
                return new CreateCommandResponse<JURemission>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the remission: " + Request.Remission.Id);
                return new CreateCommandResponse<JURemission>(new JURemission(), "Error", false, CommandEnums.CommandResultStatus.Error);
            }
        }
    }
}
