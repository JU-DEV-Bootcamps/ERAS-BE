using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Answers.Commands.CreateAnswerList
{
    public class CreateAnswerListCommand : IRequest<CreateCommandResponse<List<Answer>>>
    {
        public required List<AnswerDTO> Answers;
    }
}
