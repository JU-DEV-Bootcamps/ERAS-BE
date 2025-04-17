using Eras.Application.Dtos;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Answers.Commands.CreateAnswerList
{
    public class CreateAnswerListCommand : IRequest<CreateCommandResponse<List<Answer>>>
    {
        public List<AnswerDTO>? Answers;
    }
}
