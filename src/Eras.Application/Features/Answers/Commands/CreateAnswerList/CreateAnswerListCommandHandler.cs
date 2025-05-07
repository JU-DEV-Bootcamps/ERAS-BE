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

        public CreateAnswerListCommandHandler(IAnswerRepository AnswerRepository, ILogger<CreateAnswerListCommandHandler> Logger)
        {
            _answerRepository = AnswerRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<List<Answer>>> Handle(CreateAnswerListCommand Request, CancellationToken CancellationToken)
        {
            foreach (AnswerDTO ans in Request.Answers)
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
            var answerList = Request.Answers.Select(Ans => Ans.ToDomain()).ToList();
            return new CreateCommandResponse<List<Answer>>(answerList,"Success",true);
        }
    }
}
