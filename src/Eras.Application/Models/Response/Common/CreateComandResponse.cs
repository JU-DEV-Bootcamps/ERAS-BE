using Eras.Application.Models.Enums;
using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.Response.Common
{
    public class CreateCommandResponse<T> : BaseResponse
    {
        public T Entity { get; set; }
        public int SuccessfullImports { get; set; }
        public CommandEnums.CommandResultStatus Status { get; set; } = CommandEnums.CommandResultStatus.Success;

        public CreateCommandResponse(T CreatedEntity, int SuccessfullImports)
        {
            Entity = CreatedEntity;
            this.SuccessfullImports = SuccessfullImports;
        }
        public CreateCommandResponse(T CreatedEntity, int SuccessfullImports, string Message, bool Success) : base(Message, Success)
        {
            Entity = CreatedEntity;
            this.SuccessfullImports = SuccessfullImports;
        }

        public CreateCommandResponse(T CreatedEntity, string Message, bool Success) : base(Message, Success)
        {
            Entity = CreatedEntity;
        }

        public CreateCommandResponse(T CreatedEntity, string Message, bool Success,
            CommandEnums.CommandResultStatus Status
            ) : base(Message, Success)
        {
            Entity = CreatedEntity;
            this.Status = Status;
        }

        public CreateCommandResponse(T CreatedEntity, int SuccessfullImports, string Message, bool Success,
            CommandEnums.CommandResultStatus Status) : base(Message, Success)
        {
            Entity = CreatedEntity;
            this.SuccessfullImports = SuccessfullImports;
            this.Status = Status;
        }

    }
}
