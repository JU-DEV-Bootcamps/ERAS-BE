using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Answers.Commands.CreateAnswer
{
    public class CreateAnswerCommand : IRequest<CreateCommandResponse<Answer>>
    {
        public required AnswerDTO Answer;
    }
}
