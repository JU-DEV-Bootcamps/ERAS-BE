using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId
{
    public class GetStudentDetailByStudentIdQuery : IRequest<GetQueryResponse<StudentDetail>>
    {
        public int StudentId;
    }
}
