using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eras.Application.DTOs.CL
{
    // this is a class only to serialize from Cosmic latte
    public class CLResponseModelForAllPollsDTO
    {
        [JsonPropertyName("@data")]
        public List<DataItem> data { get; set; }

        [JsonPropertyName("@meta")]
        public MetaAllEvaluations meta { get; set; }
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
        public string _id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("parent")]
        public string parent { get; set; }

        [JsonPropertyName("configuration")]
        public Configuration configuration { get; set; }

        [JsonPropertyName("access")]
        public string access { get; set; }

        [JsonPropertyName("inventoryKey")]
        public string inventoryKey { get; set; }

        [JsonPropertyName("inventoryAccess")]
        public string inventoryAccess { get; set; }

        [JsonPropertyName("inventoryId")]
        public string inventoryId { get; set; }

        [JsonPropertyName("owner")]
        public string owner { get; set; }

        [JsonPropertyName("customFieldsSchema")]
        public List<string> customFieldsSchema { get; set; }

        [JsonPropertyName("_tenantName")]
        public string _tenantName { get; set; }

        [JsonPropertyName("changeHistory")]
        public List<ChangeHistoryItem> changeHistory { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime createdAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime updatedAt { get; set; }

        [JsonPropertyName("customFields")]
        public List<string> customFields { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("accessToken")]
        public string accessToken { get; set; }

        [JsonPropertyName("startedAt")]
        public DateTime startedAt { get; set; }

        [JsonPropertyName("elapsedTimeInSeconds")]
        public int elapsedTimeInSeconds { get; set; }

        [JsonPropertyName("finishedAt")]
        public DateTime finishedAt { get; set; }

        [JsonPropertyName("score")]
        public Score score { get; set; }
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
        public string action { get; set; }

        [JsonPropertyName("when")]
        public DateTime when { get; set; }

        [JsonPropertyName("userId")]
        public string userId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string ipAddress { get; set; }
    }

    // level 3
    public class Score
    {
        [JsonPropertyName("byPosition")]
        public List<ByPosition> byPosition { get; set; }

        [JsonPropertyName("byTrait")]
        public ByTrait byTrait { get; set; }
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
        public Dictionary<string, JsonElement> traits { get; set; }


        private static Dictionary<string, TraitData> DeserializeTraits(Dictionary<string, JsonElement> traits)
        {
            var result = new Dictionary<string, TraitData>();

            foreach (var kvp in traits)
            {
                TraitData traitData = kvp.Value.Deserialize<TraitData>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                result[kvp.Key] = traitData;
            }

            return result;
        }
        public static Dictionary<string, List<int>> getVariablesPositionByComponents(Dictionary<string, JsonElement> traits)
        {
            Dictionary<string, List<int>> componentsAndVariablesPosition = new Dictionary<string, List<int>>();

            foreach (KeyValuePair<string, TraitData> item in DeserializeTraits(traits))
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
        public Dictionary<string, Facet> Facets { get; set; }
    }

    public class Facet
    {
        public int Sum { get; set; }
        public double Avg { get; set; }
        public int Count { get; set; }
        public List<ScoreDetail> Scores { get; set; }
    }

    public class ScoreDetail
    {
        public int Score { get; set; }
        public int Position { get; set; }
    }



    /*
    // level 5
    public class Facet
    {
        [JsonPropertyName("facets")]
        public Dictionary<string, FacetValue> facets { get; set; }
    }
    public class FacetValue
    {
        [JsonPropertyName("sum")]
        public int sum { get; set; }

        [JsonPropertyName("avg")]
        public double avg { get; set; }

        [JsonPropertyName("count")]
        public int count { get; set; }

        [JsonPropertyName("min")]
        public int min { get; set; }

        [JsonPropertyName("max")]
        public int max { get; set; }

        [JsonPropertyName("scores")]
        public List<FacetValue> scores { get; set; }
    }
    */
}
