using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eras.Application.Dtos
{
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
        [JsonPropertyName("default-trait")]
        public Facet defaultTrait { get; set; }
    }

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

}
