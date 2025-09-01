using Eras.Application.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Entities;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Enums;
using Eras.Application.Contracts.Persistence;

namespace Eras.Application.Features.Professionals.Commands.CreateProfessional
{
    public class CreateProfessionalCommandHandler : IRequestHandler<CreateProfessionalCommand, CreateCommandResponse<JUProfessional>>
    {
        private readonly IProfessionalRepository _professionalRepository;
        private readonly ILogger<CreateProfessionalCommandHandler> _logger;

        public CreateProfessionalCommandHandler(IProfessionalRepository ProfessionalRepository, ILogger<CreateProfessionalCommandHandler> Logger)
        {
            _professionalRepository = ProfessionalRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<JUProfessional>> Handle(CreateProfessionalCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                int? Id = Request.Professional.Id;
                
                if (Id != null)
                {
                    JUProfessional? entityInDB = await _professionalRepository.GetByIdAsync((int) Id);
                    if (entityInDB != null) return new CreateCommandResponse<JUProfessional>(new JUProfessional(), "Entity already exists", false,
                        CommandEnums.CommandResultStatus.AlreadyExists);
                }
                
                JUProfessional professional = Request.Professional.ToDomain();
                JUProfessional response = await _professionalRepository.AddAsync(professional);
                return new CreateCommandResponse<JUProfessional>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the professional: " + Request.Professional.Id);
                return new CreateCommandResponse<JUProfessional>(new JUProfessional(), "Error", false, CommandEnums.CommandResultStatus.Error);
            }
        }
    }
}
