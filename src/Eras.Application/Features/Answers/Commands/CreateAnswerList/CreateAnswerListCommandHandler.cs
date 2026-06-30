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
            Dictionary<int, List<Answer>> answersDictionary = new Dictionary<int, List<Answer>>();
            List<Answer> newAnswers = [];
            if (answers.Count > 0)
            {
                Answer? firstAnswer = answers.FirstOrDefault();
                if (firstAnswer != null)
                {
                    List<Answer> persistedAnswersList = await _answerRepository.GetByPollInstanceIdAsync(firstAnswer.PollInstanceId);
                    answersDictionary.Add(firstAnswer.PollInstanceId, persistedAnswersList);
                }
            }

            try
            {
                foreach (Answer answer in answers)
                {
                    Answer? existingAnswer;
                    answersDictionary.TryGetValue(answer.PollInstanceId, out var answersList);

                    if (answersList != null)
                    {
                        existingAnswer = answersList.Find(
                            Ans => Ans.PollInstanceId == answer.PollInstanceId
                                && Ans.PollVariableId == answer.PollVariableId
                        );

                        if (existingAnswer != null)
                        {
                            await _answerRepository.UpdateAnswerTextAsync(existingAnswer.Id, answer.AnswerText, answer.RiskLevel);
                        }
                        else
                        {
                            newAnswers.Add(answer);
                        }
                    }
                    else
                    {
                        List<Answer> persistedAnswersList = await _answerRepository.GetByPollInstanceIdAsync(answer.PollInstanceId);
                        answersDictionary.Add(answer.PollInstanceId, persistedAnswersList);
                        newAnswers.Add(answer);
                    }
                }

                
                await _answerRepository.AddBatchAsync(newAnswers);
            }
            catch (Exception ex)
            {
                // Propagate so the surrounding import transaction rolls back instead of
                // silently committing a partial set of answers.
                _logger.LogError(ex, "An error occurred creating or updating answers");
                throw;
            }

            return new CreateCommandResponse<List<Answer>>(answers, 1, "Success", true);
        }
    }
}