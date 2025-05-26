using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Commands.CreateStudentCohort
{
    public class CreateStudentCohortCommand : IRequest<CreateCommandResponse<Student>>
    {
        public int StudentId;
        public int CohortId;
    }
}
