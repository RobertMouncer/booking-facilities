using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using booking_facilities.Models;
using Microsoft.EntityFrameworkCore;

namespace booking_facilities.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly booking_facilitiesContext context;

        public FacilityRepository(booking_facilitiesContext context)
        {
            this.context = context;
        }

        public async Task<Facility> AddAsync(Facility facility)
        {
            context.Facility.Add(facility);
            await context.SaveChangesAsync();
            return facility;
        }

        public async Task<Facility> DeleteAsync(Facility facility)
        {
            context.Facility.Remove(facility);
            await context.SaveChangesAsync();
            return facility;
        }

        public async Task<Facility> GetByIdAsync(int id)
        {
            return await context.Facility.FindAsync(id);
        }

        public async Task<Facility> UpdateAsync(Facility facility)
        {
            context.Entry(facility).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return facility;
        }

        public bool FacilityExists(int id)
        {
            return context.Facility.Any(f => f.FacilityId == id);
        }

        public IQueryable<Facility> GetAllAsync()
        {
            IQueryable<Facility> facility = context.Facility;
            return facility;
        }
    }
}
