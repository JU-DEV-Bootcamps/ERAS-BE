using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Domain.Common;

namespace Eras.Application.Models.Response.Controllers.StudentsController;
public class GetAllStudentsQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsImported { get; set; } = false;
}
