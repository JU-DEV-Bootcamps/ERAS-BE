using System.Text.Json.Serialization;

using Eras.Application.DTOs.CL;

namespace Eras.Application.DTOs.CosmicLatte;
public class CLEvaluationSetDTOList
{
    [JsonPropertyName("@data")]
    public List<CLEvaluationSetDTO> data { get; set; } = new List<CLEvaluationSetDTO>();

    [JsonPropertyName("@meta")]
    public MetaEvaluationsSet meta { get; set; } = new MetaEvaluationsSet();
}

public class CLEvaluationSetDTO {

    [JsonPropertyName("_id")]
    public string _id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string name { get; set; } = string.Empty;

    [JsonPropertyName("parent")]
    public string parent { get; set; } = string.Empty;

}

// level 1
public class MetaEvaluationsSet
{
    [JsonPropertyName("@totalCount")]
    public int totalCount { get; set; }

    [JsonPropertyName("@count")]
    public int count { get; set; }
}
