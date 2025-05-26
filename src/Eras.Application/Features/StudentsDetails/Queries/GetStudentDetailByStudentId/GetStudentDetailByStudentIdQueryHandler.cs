using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;

internal class GetStudentDetailByStudentIdQueryHandler : IRequestHandler<GetStudentDetailByStudentIdQuery, GetQueryResponse<StudentDetail?>>
{
    private readonly IStudentDetailRepository _StudentDetailRepository;
    private readonly ILogger<CreateStudentDetailCommandHandler> _Logger;
    public GetStudentDetailByStudentIdQueryHandler(IStudentDetailRepository StudentDetailRepository, ILogger<CreateStudentDetailCommandHandler> Logger)
    {
        _StudentDetailRepository = StudentDetailRepository;
        _Logger = Logger;
    }


    public async Task<GetQueryResponse<StudentDetail?>> Handle(GetStudentDetailByStudentIdQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            StudentDetail? response = await _StudentDetailRepository.GetByStudentId(Request.StudentId);
            if (response == null) {
                return new GetQueryResponse<StudentDetail?>(null, "Student Detail doesn't exist", false);
            }
            return new GetQueryResponse<StudentDetail?>(response, "Success", true);
        }
        catch (Exception ex)
        {
            _Logger.LogError($"An error occurred creating student detail {Request.StudentId}: Unexpected Error", ex.Message);
            return new GetQueryResponse<StudentDetail?>(null, "Error", false);
        }
    }
}
