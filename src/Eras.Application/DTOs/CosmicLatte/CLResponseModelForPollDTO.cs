using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.Extensions.Primitives;

namespace Eras.Application.DTOs.CL
{
    public class AnswersListConverter : JsonConverter<string[]>
    {
        public override string[] Read(ref Utf8JsonReader Reader, Type TypeToConvert, JsonSerializerOptions Options)
        {
            if (Reader.TokenType == JsonTokenType.StartArray)
            {
                var result = new List<string>();
                while (Reader.Read())
                {
                    if (Reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (Reader.TokenType == JsonTokenType.String)
                    {
                        var value = Reader.GetString();
                        if (value != null)
                            result.Add(value == "-" ? "Invalid string" : value);
                    }
                    else
                    {
                        throw new JsonException("A string value was expected in the array.");
                    }
                }
                return result.ToArray();
            }
            else if (Reader.TokenType == JsonTokenType.String)
            {
                var value = Reader.GetString();
                if (value == null)
                    return new string[] { string.Empty};
                return new string[] { value };
            }
            throw new JsonException("An array or string was expected.");
        }

        public override void Write(Utf8JsonWriter Writer, string[] Value, JsonSerializerOptions Options)
        {
            Writer.WriteStartArray();
            foreach (var item in Value)
            {
                Writer.WriteStringValue(item);
            }
            Writer.WriteEndArray();
        }
    }
    // this is a class only to serialize from Cosmic latte
    public class CLResponseModelForPollDTO
    {
        [JsonPropertyName("@data")]
        public required Data Data { get; set; }

        [JsonPropertyName("@meta")]
        public required Meta Meta { get; set; }
    }

    // level 1  
    public class Meta
    {
        [JsonPropertyName("@selfLink")]
        public required string SelfLink { get; set; }
    }
    public class Data
    {
        [JsonPropertyName("_id")]
        public required string Id { get; set; }

        [JsonPropertyName("evaluationSet")]
        public required EvaluationSet EvaluationSet { get; set; }

        [JsonPropertyName("evaluator")]
        public required Evaluator Evaluator { get; set; }

        [JsonPropertyName("evaluation")]
        public required Evaluation Evaluation { get; set; }

        [JsonPropertyName("scores")]
        public required Scores Scores { get; set; }

        [JsonPropertyName("answers")]
        public required Dictionary<int, Answers> Answers { get; set; }

        [JsonPropertyName("inventory")]
        public required Inventory Inventory { get; set; }

        [JsonPropertyName("owner")]
        public required Owner Owner { get; set; }
    }

    // level 2 (inside Data) _id, evaluationSet, evaluator, evaluation, scores, answers, inventory, owner
    public class EvaluationSet
    {
        [JsonPropertyName("_id")]
        public required string Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
    public class Evaluator
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }
    }
    public class Evaluation
    {
        [JsonPropertyName("_id")]
        public required string Id { get; set; }

        [JsonPropertyName("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("finishedAt")]
        public DateTime FinishedAt { get; set; }

        [JsonPropertyName("elapsedTimeInSeconds")]
        public int ElapsedTimeInSeconds { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
    public class Scores
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Traits { get; set; } = new();
    }

    public class Answers
    {
        [JsonPropertyName("answer")]
        [JsonConverter(typeof(AnswersListConverter))]
        public required string[] AnswersList { get; set; }

        [JsonPropertyName("question")]
        public required Question Question { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("customSettings")]
        public List<string> CustomSettings { get; set; } = new List<string>();
    }
    public class Inventory
    {
        [JsonPropertyName("_id")]
        public required string Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("key")]
        public required string Key { get; set; }

        [JsonPropertyName("access")]
        public required string Access { get; set; }
    }
    public class Owner
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }

    // level 3 (inside Answers) question 
    public class Question
    {
        [JsonPropertyName("body")]
        public required Dictionary<string, string> Body { get; set; }
    }


    // -------------------------------------------------------------------------------------------

    // level 2 (inside scores)
    public class DefaultTrait
    {
        [JsonPropertyName("avg")]
        public double Avg { get; set; }

        [JsonPropertyName("sum")]
        public int Sum { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("facets")]
        public required Facets Facets { get; set; }
    }

    // level 3 (inside scores, DefaultTrait)
    public class Facets
    {
        [JsonPropertyName("default-facet")]
        public required DefaultFacet DefaultFacet { get; set; }
    }

    // level 4 (inside scores, DefaultTrait, Facets)
    public class DefaultFacet
    {
        [JsonPropertyName("avg")]
        public double Avg { get; set; }

        [JsonPropertyName("sum")]
        public int Sum { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

}
