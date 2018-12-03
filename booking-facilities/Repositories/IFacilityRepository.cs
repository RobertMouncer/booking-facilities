using booking_facilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Repositories
{
    public interface IFacilityRepository
    {
        Task<Venue> GetByIdAsync(int id);
        Task<Venue> AddAsync(Venue venue);
        Task<Venue> UpdateAsync(Venue venue);
        Task<Venue> DeleteAsync(Venue venue);
        bool DoesFacilityExistInVenue(string name);
        bool VenueExists(int id);
        IQueryable<Facility> GetFacilityByVenueAsync(int id);
    }
}
