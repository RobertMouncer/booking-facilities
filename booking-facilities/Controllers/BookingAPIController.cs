using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;
using booking_facilities.Repositories;

namespace booking_facilities.Controllers
{
    [Authorize(Policy = "APIPolicy")]
    [Route("api/booking")]
    [ApiController]
    public class BookingAPIController : ControllerBase
    {
        private readonly IFacilityRepository facilityRepository;
        private readonly IBookingRepository bookingRepository;

        public BookingAPIController(IFacilityRepository facilityRepository, IBookingRepository bookingRepository)
        {
            this.facilityRepository = facilityRepository;
            this.bookingRepository = bookingRepository;
        }

        // GET: api/booking
        [HttpGet]
        public IEnumerable<Booking> GetBooking()
        {
            return bookingRepository.GetAllAsync().Include(b => b.Facility).Include(b => b.Facility.Venue).Include(b => b.Facility.Sport);
        }

        // GET: api/booking/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await bookingRepository.GetByIdAsync(id);

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
            var bookings = bookingRepository.GetAllAsync().Where(b => b.Facility.VenueId.Equals(venueId) 
                                                    && b.Facility.SportId.Equals(sportId) 
                                                    && !b.IsBlock
                                                    && (DateTime.Compare(b.BookingDateTime.Date, date.Date)) == 0)
                                                    .ToList();

            var blockings = bookingRepository.GetAllAsync().Where(b => b.Facility.VenueId.Equals(venueId) 
                                                    && b.Facility.SportId.Equals(sportId) 
                                                    && b.IsBlock
                                                    && DateTime.Compare(b.BookingDateTime.Date,date.Date) <= 0
                                                    && DateTime.Compare(date.Date,b.EndBookingDateTime.Date) <= 0)
                                                    .ToList();

            var facilities = facilityRepository.GetAllAsync().Where(f => f.VenueId.Equals(venueId) && f.SportId.Equals(sportId));

            var timeList = new List<DateTime>();

            int dayStartsAt = 9;
            int hoursOpen = 12;
            int[] timeSlots = new int[hoursOpen];

            foreach (Facility f in facilities)
            {
                for (int i = 0; i < hoursOpen; i++)
                {
                    foreach (Booking bloc in blockings)
                    {
                        // if block datetime starts before/at and ends after hour slot
                        // add one to this hour slot's unavailability factor
                        if((DateTime.Compare(bloc.BookingDateTime, date.AddHours(i+dayStartsAt)) <= 0) 
                            && (DateTime.Compare(date.AddHours(i+dayStartsAt), bloc.EndBookingDateTime.AddHours(-1)) <= 0)
                            && bloc.FacilityId.Equals(f.FacilityId))
                        {
                            timeSlots[i]++;
                            break;
                        }
                    }
                    foreach (Booking book in bookings)
                    {
                        if ((DateTime.Compare(book.BookingDateTime, date.AddHours(i + dayStartsAt)) == 0)
                            && book.FacilityId.Equals(f.FacilityId))
                        {
                            timeSlots[i]++;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < hoursOpen; i++)
            {
                if (timeSlots[i] < facilities.Count())
                {
                    timeList.Add(date.AddHours(i + dayStartsAt));
                } 
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

            booking.EndBookingDateTime = booking.BookingDateTime.AddHours(1);

            try
            {
                await bookingRepository.UpdateAsync(booking);
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

            booking.EndBookingDateTime = booking.BookingDateTime.AddHours(1);

            await bookingRepository.AddAsync(booking);
            var newBooking = await bookingRepository.GetAllAsync().Include(b => b.Facility.Sport)
                                                 .Include(b => b.Facility.Venue)
                                                 .Include(b => b.Facility)
                                                 .FirstOrDefaultAsync(b => b.BookingId.Equals(booking.BookingId));

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, newBooking);
        }

        // DELETE: api/booking
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            await bookingRepository.DeleteAsync(booking);

            return Ok(booking);
        }

        private bool BookingExists(int id)
        {
            return bookingRepository.GetAllAsync().Any(e => e.BookingId == id);
        }


        private int getFacility(int VenueId, int SportId, Booking booking)
        {
            var facilities = facilityRepository.GetAllAsync().Where(f => f.VenueId.Equals(VenueId) && f.SportId.Equals(SportId));

            var bookings = bookingRepository.GetAllAsync().Where(b => b.BookingDateTime.Equals(booking.BookingDateTime)
                                                         && b.Facility.VenueId.Equals(VenueId)
                                                         && b.Facility.SportId.Equals(SportId)
                                                         && !b.IsBlock);

            var blockings = bookingRepository.GetAllAsync().Where(b => b.Facility.VenueId.Equals(VenueId)
                                                         && b.Facility.SportId.Equals(SportId)
                                                         && b.IsBlock
                                                         && DateTime.Compare(b.BookingDateTime, booking.BookingDateTime) <= 0
                                                         && DateTime.Compare(booking.BookingDateTime, b.EndBookingDateTime.AddHours(-1)) <= 0);

            bool facilityTaken = false;

            foreach (Facility f in facilities)
            {
                facilityTaken = false;

                foreach (Booking book in bookings)
                {
                    if (book.FacilityId == f.FacilityId)
                    {
                        facilityTaken = true;
                        break;
                    }
                }
                if (!facilityTaken)
                {
                    foreach (Booking bloc in blockings)
                    {
                        if (bloc.FacilityId == f.FacilityId)
                        {
                            facilityTaken = true;
                            break;
                        }
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