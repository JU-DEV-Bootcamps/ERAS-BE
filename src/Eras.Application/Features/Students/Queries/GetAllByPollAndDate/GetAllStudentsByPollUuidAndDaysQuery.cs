using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Students.Queries.GetAllByPollAndDate
{
    public class GetAllStudentsByPollUuidAndDaysQuery : IRequest<PagedResult<Student>>
    {
        public Pagination Query { get; set; }
        public string PollUuid { get; set; }
        public int? Days { get; set; }
    }
}