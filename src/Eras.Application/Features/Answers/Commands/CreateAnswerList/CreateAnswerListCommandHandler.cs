using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Answers.Commands.CreateAnswerList
{
    public class CreateAnswerListCommandHandler : IRequestHandler<CreateAnswerListCommand,
        CreateCommandResponse<List<Answer>>>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly ILogger<CreateAnswerListCommandHandler> _logger;

        public CreateAnswerListCommandHandler(IAnswerRepository AnswerRepository,
            ILogger<CreateAnswerListCommandHandler> Logger)
        {
            _answerRepository = AnswerRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<List<Answer>>> Handle(CreateAnswerListCommand Request,
            CancellationToken CancellationToken)
        {
            try
            {
                List<Answer> answers = Request.Answers.Select(Ans => Ans.ToDomain()).ToList();

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
                return new CreateCommandResponse<List<Answer>>(new List<Answer>(), 0, "Error", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating answers ");
                return new CreateCommandResponse<List<Answer>>(new List<Answer>(), 0, "Error", false);
            }
        }
    }
}
