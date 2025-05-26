using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetStudentDetails
{
    public class GetStudentDetailsQueryHandler : IRequestHandler<GetStudentDetailsQuery, CreateCommandResponse<Student>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentDetailRepository _studentDetailRepository;
        private readonly ILogger<GetStudentDetailsQueryHandler> _logger;

        public GetStudentDetailsQueryHandler(IStudentRepository studentRepository, 
            ILogger<GetStudentDetailsQueryHandler> logger,
            IStudentDetailRepository studentDetailRepository)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _studentDetailRepository = studentDetailRepository;
        }


        public async Task<CreateCommandResponse<Student>> Handle(GetStudentDetailsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                Student response = await _studentRepository.GetByIdAsync(request.StudentDetailId);
                StudentDetail studentDetail = await _studentDetailRepository.GetByStudentId(response.Id);
                if(studentDetail!=null)
                    response.StudentDetail = studentDetail;
                return new CreateCommandResponse<Student>(response, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating student detail {request.StudentDetailId}: {ex.Message}");
                return new CreateCommandResponse<Student>(null, "Error", false);
            }
        }
    }
}
