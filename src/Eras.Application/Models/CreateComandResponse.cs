using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models
{
    public class CreateComandResponse<T> : BaseResponse
    {
        public T entity { get; set; }

        public CreateComandResponse(T createdEntity) 
        {
            entity = createdEntity;
        }
        public CreateComandResponse(T createdEntity, string message, bool success) : base(message, success)
        {
            entity = createdEntity;            
        }

    }
}
