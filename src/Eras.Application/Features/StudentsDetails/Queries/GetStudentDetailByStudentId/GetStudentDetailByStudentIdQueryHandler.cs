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

namespace Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId
{
    internal class GetStudentDetailByStudentIdQueryHandler : IRequestHandler<GetStudentDetailByStudentIdQuery, GetQueryResponse<StudentDetail>>
    {
        private readonly IStudentDetailRepository _studentDetailRepository;
        private readonly ILogger<CreateStudentDetailCommandHandler> _logger;
        public GetStudentDetailByStudentIdQueryHandler(IStudentDetailRepository studentDetailRepository,
            ILogger<CreateStudentDetailCommandHandler> logger)
        {
            _studentDetailRepository = studentDetailRepository;
            _logger = logger;
        }


        public async Task<GetQueryResponse<StudentDetail>> Handle(GetStudentDetailByStudentIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                StudentDetail response = await _studentDetailRepository.GetByStudentId(request.StudentId);
                if (response == null) {
                    return new GetQueryResponse<StudentDetail>(response,"Student Detail Not Exists", false);
                }
                return new GetQueryResponse<StudentDetail>(response,"Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred creating student detail {request.StudentId}: Unexpected Error");
                return new GetQueryResponse<StudentDetail>(null,"Error", false);
            }
        }
    }
}
