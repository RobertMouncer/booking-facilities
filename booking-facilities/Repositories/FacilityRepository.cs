using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using booking_facilities.Models;

namespace booking_facilities.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        public async Task<Venue> AddAsync(Venue venue)
        {
            throw new NotImplementedException();
        }

        public async Task<Venue> DeleteAsync(Venue venue)
        {
            throw new NotImplementedException();
        }

        public bool DoesFacilityExistInVenue(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<Venue> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Venue> UpdateAsync(Venue venue)
        {
            throw new NotImplementedException();
        }

        public bool VenueExists(int id)
        {
            throw new NotImplementedException();
        }
    }
}
