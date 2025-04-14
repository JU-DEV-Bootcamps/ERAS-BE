using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetByEmail
{
    public class GetStudentByEmailQuery : IRequest<GetQueryResponse<Student?>>
    {
        public string studentEmail = string.Empty;
    }
}
