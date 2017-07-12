using Newtonsoft.Json;

namespace Museum.Models
{
    public class YelpAuthenticationResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}