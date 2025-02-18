using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentsCommand: IRequest<CreateComandResponse<Student[]>>
    {
        public StudentImportDto[] students;
    }
}
