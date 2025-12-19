using System.Text.Json.Serialization;

namespace Eras.Application.DTOs.CosmicLatte
{
    public class CLResponseForAllPollsDTO
    {

        [JsonPropertyName("@data")]
        public required List<PollDataItem> data { get; set; }
    }
    public class PollDataItem
    {
        public PollDataItem(string Id, string Parent, string Name, string Status)
        {
            id = Id;
            parent = Parent;
            name = Name;
            status = Status;
        }

        [JsonPropertyName("_id")]
        public string id { get; set; }

        [JsonPropertyName("parent")]
        public string parent { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }
    }
}
