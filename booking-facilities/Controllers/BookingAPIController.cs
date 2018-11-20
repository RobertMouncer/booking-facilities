using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;

namespace booking_facilities.Controllers
{
    [Route("/booking")]
    [ApiController]
    public class BookingAPIController : ControllerBase
    {
        private readonly booking_facilitiesContext _context;

        public BookingAPIController(booking_facilitiesContext context)
        {
            _context = context;
        }

        // GET: /booking
        [HttpGet]
        public IEnumerable<Booking> GetBooking()
        {
            return _context.Booking;
        }

        // GET: /booking/5
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

        // PUT: /booking/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking([FromRoute] int id, [FromBody] Booking booking)
        {
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

            return NoContent();
        }

        // POST: /booking
        [HttpPost]
        public async Task<IActionResult> PostBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Booking.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }

        // DELETE: /booking
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
    }
}