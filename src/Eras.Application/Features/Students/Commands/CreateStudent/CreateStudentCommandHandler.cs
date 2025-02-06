using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Mappers;
using Eras.Application.Services;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, BaseResponse>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentCommandHandler> _logger;

        public CreateStudentCommandHandler(IStudentRepository studentRepository, ILogger<CreateStudentCommandHandler> logger)
        {            
            _studentRepository = studentRepository;
            _logger = logger;
        }


        public async Task<BaseResponse> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = request.student.ToDomain();

            try
            {
                var result = await _studentRepository.AddAsync(student);
                _logger.LogInformation($"Status of the creation {result}");
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred importing student with SISId {request.student.SISId}: {ex.Message}");
                return new BaseResponse(ex.Message,false);
            }
        }
    }
}
