using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using booking_facilities.Models;
using Microsoft.EntityFrameworkCore;

namespace booking_facilities.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly booking_facilitiesContext context;

        public VenueRepository(booking_facilitiesContext context)
        {
            this.context = context;
        }

        public async Task<Venue> AddAsync(Venue venue)
        {
            context.Venue.Add(venue);
            await context.SaveChangesAsync();
            return venue;
        }

        public async Task<Venue> DeleteAsync(Venue venue)
        {
            context.Venue.Remove(venue);
            await context.SaveChangesAsync();
            return venue;
        }

        public bool DoesVenueExist(string name)
        {
            if (context.Venue.Any(v => v.VenueName == name))
            {
                return true;
            }
            return false;
        }

        public IQueryable<Venue> GetAllAsync()
        {
            IQueryable<Venue> venues = context.Venue;
            return venues;
        }

        public async Task<Venue> GetByIdAsync(int id)
        {
            return await context.Venue.FindAsync(id);
        }

        public async Task<Venue> UpdateAsync(Venue venue)
        {
            context.Entry(venue).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return venue;
        }

        public bool VenueExists(int id)
        {
            return context.Venue.Any(v => v.VenueId == id);
        }
    }
}
