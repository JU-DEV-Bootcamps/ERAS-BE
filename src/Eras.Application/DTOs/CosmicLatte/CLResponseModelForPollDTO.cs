using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eras.Application.DTOs.CL;
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
                    result.Add((value == "-" || value == null) ? "Invalid string" : value);
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
            return [Reader.GetString() ?? "Null value"];
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
    public Data? Data { get; set; }

    [JsonPropertyName("@meta")]
    public Meta? Meta { get; set; }
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
    public required string _id { get; set; }

    [JsonPropertyName("evaluationSet")]
    public EvaluationSet? EvaluationSet { get; set; }

    [JsonPropertyName("evaluator")]
    public Evaluator? Evaluator { get; set; }

    [JsonPropertyName("evaluation")]
    public required Evaluation Evaluation { get; set; }

    [JsonPropertyName("scores")]
    public required Scores Scores { get; set; }

    [JsonPropertyName("answers")]
    public required Dictionary<int, Answers> Answers { get; set; }

    [JsonPropertyName("inventory")]
    public Inventory? Inventory { get; set; }

    [JsonPropertyName("owner")]
    public Owner? Owner { get; set; }
}

// level 2 (inside Data) _id, evaluationSet, evaluator, evaluation, scores, answers, inventory, owner
public class EvaluationSet
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
public class Evaluator
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
public class Evaluation
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    [JsonPropertyName("startedAt")]
    public DateTime StartedAt { get; set; }

    [JsonPropertyName("finishedAt")]
    public DateTime FinishedAt { get; set; }

    [JsonPropertyName("elapsedTimeInSeconds")]
    public int ElapsedTimeInSeconds { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
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
    public string[]? AnswersList { get; set; }

    [JsonPropertyName("question")]
    public Question? Question { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("customSettings")]
    public List<string> CustomSettings { get; set; } = new List<string>();
}
public class Inventory
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("access")]
    public string? Access { get; set; }
}
public class Owner
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
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
    public Facets? Facets { get; set; }
}

// level 3 (inside scores, DefaultTrait)
public class Facets
{
    [JsonPropertyName("default-facet")]
    public DefaultFacet? DefaultFacet { get; set; }
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
