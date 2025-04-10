using System.Text.Json.Serialization;

namespace Eras.Application.DTOs.CosmicLatte;

public class CLResponseForAllPollsDTO
{

    [JsonPropertyName("@data")]
    public List<PollDataItem>? data { get; set; }
}
public class PollDataItem(string Parent, string Name, string Status)
{
    [JsonPropertyName("parent")]
    public string parent { get; set; } = Parent;

    [JsonPropertyName("name")]
    public string name { get; set; } = Name;

    [JsonPropertyName("status")]
    public string status { get; set; } = Status;
}
