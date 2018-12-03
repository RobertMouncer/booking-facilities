using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using booking_facilities.Models;
using Microsoft.EntityFrameworkCore;

namespace booking_facilities.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        booking_facilitiesContext context;
        public BookingRepository(booking_facilitiesContext context)
        {
            this.context = context;
        }
        public IQueryable<Booking> GetAllAsync()
        {
            return context.Booking;
        }

        public async Task<Booking> DeleteAsync(Booking booking)
        {
            context.Booking.Remove(booking);
            await context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            context.Booking.Add(booking);
            await context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> GetByIdAndInclude(int id)
        {
            return await context.Booking
               .Include(b => b.Facility)
               .Include(b => b.Facility.Venue)
               .Include(b => b.Facility.Sport)
               .FirstOrDefaultAsync(m => m.BookingId == id);
        }
        public async Task<Booking> GetByIdAsync(int id)
        {
            return await context.Booking.FindAsync(id);
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            context.Update(booking);
            await context.SaveChangesAsync();
            return booking;
        }
        public IQueryable<Booking> GetBookingsInLocationAtDateTime(Booking booking, int VenueId, int SportId)
        {
            return context.Booking.Where(b => b.BookingDateTime.Equals(booking.BookingDateTime) 
                                            && b.Facility.VenueId.Equals(VenueId) 
                                            && b.Facility.SportId.Equals(SportId));
        }

    }
}
