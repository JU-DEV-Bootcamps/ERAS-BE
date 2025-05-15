using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.CL;
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
            List<Answer> answers = Request.Answers.Select(Ans => Ans.ToDomain()).ToList();
            foreach (Answer answer in answers)
            {
                try
                {
                    await _answerRepository.AddAsync(answer);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred creating answers ");
                }
            }
            return new CreateCommandResponse<List<Answer>>(answers, 1, "Success", true);

        }
    }
}