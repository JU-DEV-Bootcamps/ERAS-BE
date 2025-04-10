using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models;
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

        public CreateAnswerCommandHandler(IAnswerRepository answerRepository, ILogger<CreateAnswerCommandHandler> logger)
        {
            _answerRepository = answerRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<Answer>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Answer answer = request.Answer.ToDomain();
                Answer createdAnswer = await _answerRepository.AddAsync(answer);
                return new CreateCommandResponse<Answer>(createdAnswer, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the Answer: " + request.Answer.Answer);
                return new CreateCommandResponse<Answer>(null, 0, "Error", false);
            }
        }
    }
}
