using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Eras.Application.Features.Students.Queries.GetStudentDetails
{
    public class GetStudentDetailsQueryHandler : IRequestHandler<GetStudentDetailsQuery, CreateCommandResponse<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentDetailRepository _studentDetailRepository;
        private readonly ILogger<GetStudentDetailsQueryHandler> _logger;

        public GetStudentDetailsQueryHandler(IStudentRepository StudentRepository, 
            ILogger<GetStudentDetailsQueryHandler> Logger,
            IStudentDetailRepository StudentDetailRepository)
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
            _studentDetailRepository = StudentDetailRepository;
        }


        public async Task<CreateCommandResponse<Student>> Handle(GetStudentDetailsQuery Request, CancellationToken CancellationToken)
        {

            try
            {
                if (Request.StudentDetailId == null)
                {
                    _logger.LogError($"An error occurred creating student detail Request.StudentDetailId is null");
                    return new CreateCommandResponse<Student>(null, 0, "Error", false);
                }

                Student response = await _studentRepository.GetByIdAsync(Request.StudentDetailId.Value);

                if (response == null)
                    return new CreateCommandResponse<Student>(new Student(), "Student Not Found", true,
                        Models.Enums.CommandEnums.CommandResultStatus.NotFound);

                StudentDetail studentDetail = await _studentDetailRepository.GetByStudentId(response.Id);

                if (studentDetail != null)
                    response.StudentDetail = studentDetail;
                return new CreateCommandResponse<Student>(response, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating student detail {Request.StudentDetailId}: {ex.Message}");
                return new CreateCommandResponse<Student>(null, 0, "Error", false);
            }
        }
    }
}
