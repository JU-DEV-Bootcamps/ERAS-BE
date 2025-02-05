using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Mappers;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentsCommandHandler : IRequestHandler<CreateStudentsCommand, BaseResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentsCommandHandler> _logger;
        private readonly IMediator _mediator;

        public CreateStudentsCommandHandler(IStudentRepository studentRepository, ILogger<CreateStudentsCommandHandler> logger, IMediator mediator)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<BaseResponse> Handle(CreateStudentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Importing students");
                foreach (var dto in request.students)
                {
                    CreateStudentCommand createStudentCommand = new CreateStudentCommand() { student = dto };
                    await _mediator.Send(createStudentCommand);
                }

                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the massive import process");
                return new BaseResponse(false);
            }
        }
    }
}
