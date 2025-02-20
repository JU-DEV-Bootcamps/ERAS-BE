using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Answers.Commands.CreateAnswer;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Answers.Commands.CreateAnswerList
{
    public class CreateAnswerListCommandHandler : IRequestHandler<CreateAnswerListCommand, CreateComandResponse<List<Answer>>>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly ILogger<CreateAnswerListCommandHandler> _logger;

        public CreateAnswerListCommandHandler(IAnswerRepository answerRepository, ILogger<CreateAnswerListCommandHandler> logger)
        {
            _answerRepository = answerRepository;
            _logger = logger;
        }

        public async Task<CreateComandResponse<List<Answer>>> Handle(CreateAnswerListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<Answer> answers = request.Answers.Select(ans => ans.ToDomain()).ToList();

                await _answerRepository.SaveManyAnswersAsync(answers);

                return new CreateComandResponse<List<Answer>>(answers, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating answers ");
                return new CreateComandResponse<List<Answer>>(null, 0, "Error", false);
            }
        }
    }
}
