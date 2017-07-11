using Newtonsoft.Json;
using System;

namespace Museum.Models
{
    public class Places
    {
        [JsonProperty("Location")]
        public string Location { get; private set; }

        [JsonProperty("PickedBy")]
        public string PickedBy { get; private set; }

        [JsonProperty("Date")]
        public DateTime Date { get; private set; }
    }
}