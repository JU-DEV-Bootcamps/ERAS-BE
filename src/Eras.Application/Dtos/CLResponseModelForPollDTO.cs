using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eras.Application.Dtos
{
    // this is a class only to serialize from Cosmic latte
    public class CLResponseModelForPollDTO
    {
        [JsonPropertyName("@data")]
        public Data Data { get; set; }

        [JsonPropertyName("@meta")]
        public Meta Meta { get; set; }
    }

    // level 1  
    public class Meta
    {
        [JsonPropertyName("@selfLink")]
        public string SelfLink { get; set; }
    }
    public class Data
    {
        [JsonPropertyName("_id")]
        public string _id { get; set; }

        [JsonPropertyName("evaluationSet")]
        public EvaluationSet EvaluationSet { get; set; }

        [JsonPropertyName("evaluator")]
        public Evaluator Evaluator { get; set; }

        [JsonPropertyName("evaluation")]
        public Evaluation Evaluation { get; set; }

        [JsonPropertyName("scores")]
        public Scores Scores { get; set; }

        [JsonPropertyName("answers")]
        public Dictionary<int, Answers> Answers { get; set; }

        [JsonPropertyName("inventory")]
        public Inventory Inventory { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; }
    }

    // level 2 (inside Data) _id, evaluationSet, evaluator, evaluation, scores, answers, inventory, owner
    public class EvaluationSet
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
    public class Evaluator
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
    public class Evaluation
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("finishedAt")]
        public DateTime FinishedAt { get; set; }

        [JsonPropertyName("elapsedTimeInSeconds")]
        public int ElapsedTimeInSeconds { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
    public class Scores
    {
        [JsonPropertyName("default-trait")]
        public DefaultTrait DefaultTrait { get; set; }
    }
    public class Answers
    {
        [JsonPropertyName("answer")]
        public string[] AnswersList { get; set; }

        [JsonPropertyName("question")]
        public Question Question { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("customSettings")]
        public List<string> CustomSettings { get; set; } = new List<string>();
    }
    public class Inventory
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("access")]
        public string Access { get; set; }
    }
    public class Owner
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    // level 3 (inside Answers) question 
    public class Question
    {
        [JsonPropertyName("body")]
        public Dictionary<string, string> Body { get; set; }
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
        public Facets Facets { get; set; }
    }

    // level 3 (inside scores, DefaultTrait)
    public class Facets
    {
        [JsonPropertyName("default-facet")]
        public DefaultFacet DefaultFacet { get; set; }
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
