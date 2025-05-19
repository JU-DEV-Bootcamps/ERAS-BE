using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.Enums;
public class CommandEnums
{
    public enum CommandResultStatus
    {
        Success,
        NotFound,
        Error,
        AlreadyExists
    }
}
