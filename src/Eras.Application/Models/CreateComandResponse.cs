using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Eras.Application.Models
{
    public class CreateCommandResponse<T> : BaseResponse
    {
        public T Entity { get; set; }
        public int SuccessfullImports { get; set; }

        public CreateCommandResponse(T createdEntity, int successfullImports)
        {
            Entity = createdEntity;
            SuccessfullImports = successfullImports;
        }
        public CreateCommandResponse(T createdEntity, int successfullImports, string message, bool success) : base(message, success)
        {
            Entity = createdEntity;
            SuccessfullImports = successfullImports;
        }

        public CreateCommandResponse(T createdEntity, string message, bool success) : base(message, success)
        {
            Entity = createdEntity;
        }

    }
}
