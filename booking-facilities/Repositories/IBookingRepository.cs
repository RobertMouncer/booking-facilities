using booking_facilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Repositories
{
    public interface IBookingRepository
    {
        IQueryable<Booking> GetAllAsync();
        Task<Booking> DeleteAsync(Booking booking);
        Task<Booking> AddAsync(Booking booking);
        Task<Booking> GetByIdAsync(int id);
        Task<Booking> UpdateAsync(Booking booking);
        IQueryable<Booking> GetBookingsInLocationAtDateTime(DateTime datetime, int VenueId, int SportId);
    }
}
