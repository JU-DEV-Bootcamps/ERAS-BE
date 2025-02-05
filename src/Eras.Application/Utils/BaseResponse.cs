using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Utils
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? ValidationErrors { get; set; }

        public BaseResponse(bool status)
        {
            Success = status;
        }

        public BaseResponse(string message)
        {
            Success = true;
            message = message;
        }

        public BaseResponse(string message, bool success)
        {
            Success = success;
            Message = message;
        }

    }
}
