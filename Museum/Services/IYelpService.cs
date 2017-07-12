
using System.Threading.Tasks;
using Museum.Models;

namespace Museum.Services
{
    interface IYelpService
    {
        Task<YelpBusiness> GetRandomUnvisitedPlacesAsync(Places[] previouslyVisitedPlaces);
    }
}
