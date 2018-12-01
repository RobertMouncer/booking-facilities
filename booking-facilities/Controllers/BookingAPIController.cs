using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;

namespace booking_facilities.Controllers
{
    [Authorize(Policy = "APIPolicy")]
    [Route("api/booking")]
    [ApiController]
    public class BookingAPIController : ControllerBase
    {
        private readonly booking_facilitiesContext _context;

        public BookingAPIController(booking_facilitiesContext context)
        {
            _context = context;
        }

        // GET: api/booking
        [HttpGet]
        public IEnumerable<Booking> GetBooking()
        {
            return _context.Booking;
        }

        // GET: api/booking/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // GET: api/booking/2018-11-25/1/2
        [HttpGet("{date}/{venueId}/{sportId}")]
        public IActionResult GetTimes([FromRoute] DateTime date, [FromRoute] int venueId, [FromRoute] int sportId)
        {
            //TO-DO check admin times - remove from list

            //get list of bookings to check for available times
            var bookings = _context.Booking.Where(b => b.Facility.VenueId.Equals(venueId) && b.Facility.SportId.Equals(sportId) && (DateTime.Compare(b.BookingDateTime.Date, date.Date)) == 0).ToList();
            //get list of facilites to check for available facility for this sport and venue
            var facilities = _context.Facility.Where(f => f.VenueId.Equals(venueId) && f.SportId.Equals(sportId));
            var timeList = new List<DateTime>();
            int dayStartsAt = 9;
            int hoursOpen = 12;
            int facilitiesLength = facilities.Count();
            int countBookings = 0;

            date = date.AddHours(dayStartsAt);

            //iterate through each hour that the facility can be open
            //count number of bookings for each facility -> if count is equal to number of facilites then this time is not available
            for (int i = 0; i < hoursOpen; i++)
            {
                countBookings = bookings.Count(b => b.BookingDateTime.Equals(date));
                //if count is less than number of facilites then this time is available
                if (countBookings < facilitiesLength)
                {
                    timeList.Add(date);
                }
                date = date.AddHours(1);
            }

            return Ok(timeList);
        }


        // PUT: api/booking/5
        [HttpPut("{id}/{venueId}/{sportId}")]
        public async Task<IActionResult> PutBooking([FromRoute] int id, [FromRoute] int venueId, [FromRoute] int sportId, [FromBody] Booking booking)
        {
            int facilityId = getFacility(venueId, sportId, booking);

            if (DateTime.Compare(booking.BookingDateTime, DateTime.Now) <= 0)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is in the past. Please enter future Date/Time.");
            }
            if (facilityId == -1)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is no longer available. Please try again.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != booking.BookingId)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(booking);
        }

        // POST: api/booking
        [HttpPost("{venueId}/{sportId}")]
        public async Task<IActionResult> PostBooking([FromRoute] int venueId, [FromRoute] int sportId, [FromBody] Booking booking)
        {
            booking.FacilityId = getFacility(venueId, sportId, booking);
            

            if (DateTime.Compare(booking.BookingDateTime, DateTime.Now) <= 0)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is in the past. Please enter future Date/Time.");
            }
            if (booking.FacilityId == -1)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is no longer available. Please try again.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }

        // DELETE: api/booking
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }

        private int getFacility(int venueId, int sportId, Booking booking)
        {
            var facilities = _context.Facility.Where(f => f.VenueId.Equals(venueId) && f.SportId.Equals(sportId));
            var bookings = _context.Booking.Where(b => b.BookingDateTime.Equals(booking.BookingDateTime)
                                                         && b.Facility.VenueId.Equals(venueId)
                                                         && b.Facility.SportId.Equals(sportId));
            bool facilityTaken = false;

            foreach (Facility f in facilities)
            {
                facilityTaken = false;

                foreach (Booking b in bookings)
                {
                    if (b.FacilityId == f.FacilityId)
                    {
                        facilityTaken = true;
                        break;
                    }
                }
                if (!facilityTaken)
                {
                    return f.FacilityId;
                }
            }

            return -1;
        }
    }
}