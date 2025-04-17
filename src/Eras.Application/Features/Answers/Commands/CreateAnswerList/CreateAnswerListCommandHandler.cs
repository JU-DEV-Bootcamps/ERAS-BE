using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Answers.Commands.CreateAnswerList
{
    public class CreateAnswerListCommandHandler : IRequestHandler<CreateAnswerListCommand, CreateCommandResponse<List<Answer>>>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly ILogger<CreateAnswerListCommandHandler> _logger;

        public CreateAnswerListCommandHandler(IAnswerRepository answerRepository, ILogger<CreateAnswerListCommandHandler> logger)
        {
            _answerRepository = answerRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<List<Answer>>> Handle(CreateAnswerListCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<Answer> answers = request.Answers.Select(ans => ans.ToDomain()).ToList();

                await _answerRepository.SaveManyAnswersAsync(answers);

                return new CreateCommandResponse<List<Answer>>(answers, 1, "Success", true);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException!=null)
                {
                    _logger.LogError(ex.InnerException.Message, "Create error on Answer");
                }
                else
                    _logger.LogError(ex.Message, "Create error on Answer");
                return new CreateCommandResponse<List<Answer>>(null, 0, "Error", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating answers ");
                return new CreateCommandResponse<List<Answer>>(null, 0, "Error", false);
            }
        }
    }
}
