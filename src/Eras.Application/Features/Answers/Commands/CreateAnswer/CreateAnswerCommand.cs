using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Dtos;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Answers.Commands.CreateAnswer
{
    public class CreateAnswerCommand : IRequest<CreateCommandResponse<Answer>>
    {
        public AnswerDTO? Answer;
    }
}
