using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, CreateComandResponse<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CreateStudentCommandHandler> _logger;

        public CreateStudentCommandHandler(IStudentRepository studentRepository, ILogger<CreateStudentCommandHandler> logger)
        {            
            _studentRepository = studentRepository;
            _logger = logger;
        }


        public async Task<CreateComandResponse<Student>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {

            try
            {
                Student student = request.StudentDTO.ToDomain();
                Student studentCreated = await _studentRepository.AddAsync(student);
                return new CreateComandResponse<Student>(studentCreated,1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred importing student with SISId {request.StudentDTO.SISId}: {ex.Message}");
                return new CreateComandResponse<Student>(null,0, "Error", false);
            }
        }
    }
}
