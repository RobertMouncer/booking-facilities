using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;

namespace booking_facilities.Controllers
{
    [Authorize(AuthenticationSchemes = "oidc")]
    public class BookingsController : Controller
    {
        private readonly booking_facilitiesContext _context;

        public BookingsController(booking_facilitiesContext context)
        {
            _context = context;
        }


        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var booking_facilitiesContext = _context.Booking.Include(b => b.Facility).Include(b => b.Facility.Venue).Include(b => b.Facility.Sport);
            return View(await booking_facilitiesContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Facility)
                .Include(b => b.Facility.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName");
            ViewData["FacilityId"] = new SelectList(_context.Facility, "FacilityId", "FacilityName");
            ViewData["SportId"] = new SelectList(_context.Sport, "SportId", "SportName");
            return View();
        }
        


        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,FacilityId,BookingDateTime,UserId")] Booking booking, [Bind("VenueId")] int VenueId, [Bind("SportId")] int SportId)
        {
            //Find facility that can be booked.
            var bookings = _context.Booking.Where(b => b.BookingDateTime.Equals(booking.BookingDateTime) && b.Facility.VenueId.Equals(VenueId) && b.Facility.SportId.Equals(SportId));
            var facilities = _context.Facility;
            var faciltiesFiltered = facilities.Where(f => f.VenueId.Equals(VenueId) && f.SportId.Equals(SportId));
            
            bool facilityTaken = false;

            foreach (Facility f in faciltiesFiltered) //loop around three facilities
            {
                facilityTaken = false;
                
                foreach (Booking b in bookings)// 2 (court 1 and court 2)
                {
                    if (b.FacilityId == f.FacilityId) // if the booking facility id is equal to the facility id, the facility is taken
                    {
                        facilityTaken = true;
                        break;
                    }
                }
                if(!facilityTaken) // if the facilty is not taken -> book that facility
                {
                    booking.FacilityId = f.FacilityId;
                    break;
                }
            }
            //adds model errors if date input is not correct
            //checks date/time to be booked is after current date/time -> else add model error.
            //and adds model error if facilities are not available. 
             if (DateTime.Compare(booking.BookingDateTime, DateTime.Now) <= 0)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is in the past. Please enter future Date/Time.");
            }
            else if (facilityTaken)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is no longer available. Please try again.");
            }
            else if(ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName");
            ViewData["FacilityId"] = new SelectList(facilities, "FacilityId", "FacilityName");
            ViewData["SportId"] = new SelectList(_context.Sport, "SportId", "SportName");
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["FacilityId"] = new SelectList(_context.Facility, "FacilityId", "FacilityName", booking.FacilityId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,FacilityId,BookingDateTime,UserId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facility, "FacilityId", "FacilityName", booking.FacilityId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Facility)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}
