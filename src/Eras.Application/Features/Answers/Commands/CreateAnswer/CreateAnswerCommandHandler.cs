using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Answers.Commands.CreateAnswer
{
    public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, CreateCommandResponse<Answer>>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly ILogger<CreateAnswerCommandHandler> _logger;

        public CreateAnswerCommandHandler(IAnswerRepository AnswerRepository, ILogger<CreateAnswerCommandHandler> Logger)
        {
            _answerRepository = AnswerRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Answer>> Handle(CreateAnswerCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                Answer answer = Request.Answer.ToDomain();
                Answer createdAnswer = await _answerRepository.AddAsync(answer);
                return new CreateCommandResponse<Answer>(createdAnswer, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the Answer: " + Request.Answer.Answer);
                return new CreateCommandResponse<Answer>(new Answer(), 0, "Error", false);
            }
        }
    }
}
