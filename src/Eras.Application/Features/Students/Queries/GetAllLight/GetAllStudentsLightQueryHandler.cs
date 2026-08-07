using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.DTOs.Student;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Students.Queries.GetAllLight
{
    public class GetAllStudentsLightQueryHandler
        : IRequestHandler<GetAllStudentsLightQuery, List<StudentLightDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<GetAllStudentsLightQueryHandler> _logger;

        public GetAllStudentsLightQueryHandler(
            IStudentRepository StudentRepository,
            ILogger<GetAllStudentsLightQueryHandler> Logger)
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
        }

        public async Task<List<StudentLightDto>> Handle(
            GetAllStudentsLightQuery Request,
            CancellationToken CancellationToken)
        {
            try
            {
                var students = await _studentRepository.GetAllLightAsync();
                return students.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting light students list: " + ex.Message);
                return [];
            }
        }
    }
}