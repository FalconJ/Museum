using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Museum.Models;

namespace Museum.Services
{
    public class FirebaseService : IFirebaseService
    {
        private HttpClient _client;

        public FirebaseService(string firebaseEndpoint)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(firebaseEndpoint)
            };
        }

        public async Task<Places[]> GetAllVisitedLocationsAsync()
        {
            var json = await _client.GetStringAsync("/Places/.json");

            return JsonConvert.DeserializeObject<Places[]>(json);
        }

        public async Task<Places> GetLastVisitedLocationAsync()
        {
            var places = await GetAllVisitedLocationsAsync();
            return places.LastOrDefault();
        }

        public async Task<string> GetPreviouslyVisitedLocationsMessageAsync()
        {
            try
            {
                var places = await GetAllVisitedLocationsAsync();
                var msg = new StringBuilder(Messages.LocationListingMessage);

                foreach(var place in places)
                {
                    msg.AppendLine($"- '{place.Location}' on {place.Date.ToString("dd/MM/yyyy")} ({place.PickedBy})");
                }

                return msg.ToString();
            }
            catch
            {
                return Messages.DatabaseAccessIssuesMessage;
            }
        }

        public async Task<string[]> GetAllMembersAsync()
        {
            var json = await _client.GetStringAsync("/Members/.json");
            return JsonConvert.DeserializeObject<string[]>(json);
        }
    }
}