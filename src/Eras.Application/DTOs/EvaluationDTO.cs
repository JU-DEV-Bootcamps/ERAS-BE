using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Eras.Domain.Common;

namespace Eras.Application.DTOs
{
    public class EvaluationDTO
    {
        [JsonPropertyName(nameof(Id))]
        public int Id { get; set; }
        [JsonPropertyName(nameof(Name))]
        public required string Name { get; set; }

        [JsonPropertyName(nameof(StartDate))]

        public required DateTime StartDate { get; set; }

        [JsonPropertyName(nameof(EndDate))]
        public required DateTime EndDate { get; set; }

        [JsonPropertyName(nameof(PollName))]
        public string PollName { get; set; } = string.Empty;

        [JsonPropertyName(nameof(EvaluationPollId))]
        public int EvaluationPollId { get; set; }

        public int pollId { get; set; }
        public string status { get; set; } = String.Empty;

    }
}
