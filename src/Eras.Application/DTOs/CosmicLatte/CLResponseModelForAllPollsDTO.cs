using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eras.Application.DTOs.CL
{
    // this is a class only to serialize from Cosmic latte
    public class CLResponseModelForAllPollsDTO
    {
        [JsonPropertyName("@data")]
        public List<DataItem> data { get; set; } = new List<DataItem>();

        [JsonPropertyName("@meta")]
        public MetaAllEvaluations meta { get; set; } = new MetaAllEvaluations();
    }

    // level 1
    public class MetaAllEvaluations
    {
        [JsonPropertyName("@totalCount")]
        public int totalCount { get; set; }

        [JsonPropertyName("@count")]
        public int count { get; set; }
    }
    public class DataItem
    {
        [JsonPropertyName("_id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public required string name { get; set; }

        [JsonPropertyName("parent")]
        public required string parent { get; set; }

        [JsonPropertyName("configuration")]
        public required Configuration configuration { get; set; }

        [JsonPropertyName("access")]
        public required string access { get; set; }

        [JsonPropertyName("inventoryKey")]
        public required string inventoryKey { get; set; }

        [JsonPropertyName("inventoryAccess")]
        public required string inventoryAccess { get; set; }

        [JsonPropertyName("inventoryId")]
        public required string inventoryId { get; set; }

        [JsonPropertyName("owner")]
        public required string owner { get; set; }

        [JsonPropertyName("customFieldsSchema")]
        public required List<string> customFieldsSchema { get; set; }

        [JsonPropertyName("_tenantName")]
        public required string TenantName { get; set; }

        [JsonPropertyName("changeHistory")]
        public required List<ChangeHistoryItem> changeHistory { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime updatedAt { get; set; }

        [JsonPropertyName("customFields")]
        public required List<string> customFields { get; set; }

        [JsonPropertyName("status")]
        public required string status { get; set; }

        [JsonPropertyName("accessToken")]
        public required string accessToken { get; set; }

        [JsonPropertyName("startedAt")]
        public DateTime startedAt { get; set; }

        [JsonPropertyName("elapsedTimeInSeconds")]
        public int elapsedTimeInSeconds { get; set; }

        [JsonPropertyName("finishedAt")]
        public DateTime finishedAt { get; set; }

        [JsonPropertyName("score")]
        public Score? score { get; set; }
    }

    // level 2
    public class Configuration
    {
        [JsonPropertyName("grantPublicAccessToScore")]
        public bool grantPublicAccessToScore { get; set; }
    }
    public class ChangeHistoryItem
    {
        [JsonPropertyName("action")]
        public required string action { get; set; }

        [JsonPropertyName("when")]
        public DateTime when { get; set; }

        [JsonPropertyName("userId")]
        public required string userId { get; set; }

        [JsonPropertyName("ipAddress")]
        public required string ipAddress { get; set; }
    }

    // level 3
    public class Score
    {
        [JsonPropertyName("byPosition")]
        public required List<ByPosition> byPosition { get; set; }

        [JsonPropertyName("byTrait")]
        public required ByTrait byTrait { get; set; }
    }

    // level 4
    public class ByPosition
    {
        [JsonPropertyName("score")]
        public double score { get; set; }

        [JsonPropertyName("position")]
        public int position { get; set; }
    }
    public class ByTrait
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement> Traits { get; set; } = [];


        private static Dictionary<string, TraitData> DeserializeTraits(Dictionary<string, JsonElement> Traits)
        {
            var result = new Dictionary<string, TraitData>();

            foreach (var kvp in Traits)
            {
                TraitData? traitData = kvp.Value.Deserialize<TraitData>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (traitData != null)
                    result[kvp.Key] = traitData;
            }

            return result;
        }
        public static Dictionary<string, List<int>> getVariablesPositionByComponents(Dictionary<string, JsonElement> Traits)
        {
            Dictionary<string, List<int>> componentsAndVariablesPosition = new Dictionary<string, List<int>>();

            foreach (KeyValuePair<string, TraitData> item in DeserializeTraits(Traits))
            {
                string componentName = item.Key;
                List<int> variablePositions = new List<int>();

                Dictionary<string, Facet> facets = item.Value.Facets;
                foreach (KeyValuePair<string, Facet> facet in facets)
                {
                    List<ScoreDetail> scores = facet.Value.Scores;
                    foreach (ScoreDetail score in scores)
                    {
                        variablePositions.Add(score.Position);
                    }
                }
                componentsAndVariablesPosition.Add(componentName, variablePositions);
            }
            return componentsAndVariablesPosition;
        }
    }
    public class TraitData
    {
        public int Sum { get; set; }
        public double Avg { get; set; }
        public int Count { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public required Dictionary<string, Facet> Facets { get; set; }
    }

    public class Facet
    {
        public int Sum { get; set; }
        public double Avg { get; set; }
        public int Count { get; set; }
        public required List<ScoreDetail> Scores { get; set; }
    }

    public class ScoreDetail
    {
        public int Score { get; set; }
        public int Position { get; set; }
    }
}
