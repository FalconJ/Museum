using Newtonsoft.Json;

namespace Museum.Models
{
    public class YelpSearchResponse
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("businesses")]
        public YelpBusiness[] Places { get; set; }
    }
}