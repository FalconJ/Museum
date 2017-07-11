using System;
using System.Threading.Tasks;
using Museum.Models;

namespace Museum.Services
{
    interface IFirebaseService
    {
        Task<Places[]> GetAllVisitedLocationsAsync();
        Task<Places> GetLastVisitedLocationAsync();
        Task<string> GetPreviouslyVisitedLocationsMessageAsync();
        Task<string[]> GetAllMembersAsync();
    }
}