using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Answers.Commands.CreateAnswer
{
    public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, BaseResponse>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly ILogger<CreateAnswerCommandHandler> _logger;

        public CreateAnswerCommandHandler(IAnswerRepository answerRepository, ILogger<CreateAnswerCommandHandler> logger)
        {
            _answerRepository = answerRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // var answer = CosmicLatteMapper.DtoToAnswer(request.answer);
                // await _answerRepository.AddAsync(answer);
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the Answer: " + request.answer.Answer);
                return new BaseResponse(false);
            }
        }
    }
}
