using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Eras.Application.DTOs.CosmicLatte
{
    public class CLResponseForAllPollsDTO
    {

        [JsonPropertyName("@data")]
        public required List<PollDataItem> data { get; set; }
    }
    public class PollDataItem
    {
        public PollDataItem(string Parent, string Name, string Status)
        {
            this.parent = Parent;
            this.name = Name;
            this.status = Status;
        }

        [JsonPropertyName("parent")]
        public string parent { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }
    }
}
