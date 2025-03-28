﻿using Eras.Application.Dtos;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class AnswerMapper
    {
        public static Answer ToDomain(this AnswerDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            return new Answer
            {
                AnswerText = dto.Answer,
                RiskLevel = (int) dto.Score,
                PollInstanceId = dto.PollInstanceId,
                PollVariableId = dto.PollVariableId,
                Audit = dto.Audit
            };
        }
        public static AnswerDTO ToDto(this Answer domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            return new AnswerDTO
            {
                Answer = domain.AnswerText,
                Score = domain.RiskLevel,
                PollInstanceId = domain.PollInstanceId,
                PollVariableId = domain.PollVariableId,
                Audit = domain.Audit
            };
        }
    }
}