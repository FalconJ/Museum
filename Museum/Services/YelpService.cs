using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Museum.Models;


namespace Museum.Services
{
    public class YelpService : IYelpService
    {
        private const string YelpSearchUrl = "https://api.yelp.com/v3/businesses/search?";

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _preferrredLocation;
        private string _authToken;

        public YelpService(string clientId, string clientSecret, string preferredLocation = "Monterrey")
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _preferrredLocation = preferredLocation;
        }

        /*
         * Get random unvisited places from Yelp API
         */
        public async Task<YelpBusiness> GetRandomUnvisitedPlacesAsync(Places[] placesToExclude)
        {
            try
            {
                using(var yelpClient = new HttpClient())
                {
                    await EnsureYelpAuthenticationAsync(yelpClient);

                    if (String.IsNullOrWhiteSpace(_authToken))
                    {
                        // Something went wrong during auth process
                        return null;
                    }

                    var response = await GetYelpSearchQueryAsync(yelpClient);
                    var recommendations = response.Places
                        .OrderBy(r => Guid.NewGuid())
                        .First(r => placesToExclude.All(v => !v.Location.Contains(r.Name) && !r.Name.Contains(v.Location)));

                    return recommendations;
                }
            }
            catch
            {
                // If something went wrong again
                return null;
            }
        }

        /*
         * Get access token from Yelp
         */

        private async Task EnsureYelpAuthenticationAsync(HttpClient yelpClient)
        {
            if (string.IsNullOrWhiteSpace(_authToken))
            {
                var authenticationResponse = await yelpClient.PostAsync($"https://api.yelp.com/oauth2/token?client_id={_clientId}&client_secret={_clientSecret}&grant_type=client_credentials", null);
                if (authenticationResponse.IsSuccessStatusCode)
                {
                    var authResponse = JsonConvert.DeserializeObject<YelpAuthenticationResponse>(await authenticationResponse.Content.ReadAsStringAsync());
                    _authToken = authResponse.AccessToken;
                }
            }
        }

        /*
         * Set the search words for the query
         */
        private async Task<YelpSearchResponse> GetYelpSearchQueryAsync(HttpClient yelpClient)
        {
            yelpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");
            var searchTerms = new[]
            {
                $"term=food",
                $"location={_preferrredLocation}",
                $"limit=50"
            };

            var searchRequest = await yelpClient.GetStringAsync($"{YelpSearchUrl}{string.Join("&", searchTerms)}");
            return JsonConvert.DeserializeObject<YelpSearchResponse>(searchRequest);
        }
    }
}