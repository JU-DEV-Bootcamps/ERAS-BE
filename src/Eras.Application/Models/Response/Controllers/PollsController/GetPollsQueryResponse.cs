using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Application.Models.Response.Controllers.PollsController;
public class GetPollsQueryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
    public int LastVersion { get; set; }
    public DateTime LastVersionDate { get; set; }
}
