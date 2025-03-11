using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetStudentData
{
    public class GetStudentDataQuery : IRequest<CreateComandResponse<Student>>
    {
    }
}
