using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;

public class GetStudentDetailByStudentIdQuery : IRequest<GetQueryResponse<StudentDetail?>>
{
    public int StudentId;
}
