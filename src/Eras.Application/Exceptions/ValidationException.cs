using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> ValidationErrors { get; set; }
        public ValidationException(ValidationResult validationResult)
        {
            ValidationErrors = new List<string>();
            foreach(var validationError in validationResult.Errors)
            {

            }
        }
    }
}
