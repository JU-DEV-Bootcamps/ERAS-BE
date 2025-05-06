using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
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
            foreach (AnswerDTO ans in request.Answers)
            {
                try
                {
                    Answer answer = ans.ToDomain();
                    await _answerRepository.AddAsync(answer);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null)
                    {
                        _logger.LogError(ex.InnerException.Message, "Create error on Answer");
                    }
                    else
                        _logger.LogError(ex.Message, "Create error on Answer");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred creating answers ");
                }
            }
            var answerList = request.Answers.Select(Ans => Ans.ToDomain()).ToList();
            return new CreateCommandResponse<List<Answer>>(answerList,"Success",true);
        }
    }
}
