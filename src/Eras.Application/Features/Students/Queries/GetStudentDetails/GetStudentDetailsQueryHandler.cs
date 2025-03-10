using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetStudentDetails
{
    public class GetStudentDetailsQueryHandler : IRequestHandler<GetStudentDetailsQuery, CreateComandResponse<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<GetStudentDetailsQueryHandler> _logger;

        public GetStudentDetailsQueryHandler(IStudentRepository studentRepository, ILogger<GetStudentDetailsQueryHandler> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }


        public async Task<CreateComandResponse<Student>> Handle(GetStudentDetailsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                Student response = await _studentRepository.GetByIdAsync(request.StudentDetailId);
                return new CreateComandResponse<Student>(response, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating student detail {request.StudentDetailId}: {ex.Message}");
                return new CreateComandResponse<Student>(null, "Error", false);
            }
        }
    }
}
