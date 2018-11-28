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
    public class FacilitiesController : Controller
    {
        private readonly booking_facilitiesContext _context;

        public FacilitiesController(booking_facilitiesContext context)
        {
            _context = context;
        }

        // GET: Facilities
        public async Task<IActionResult> Index()
        {
            var booking_facilitiesContext = _context.Facility.Include(f => f.Sport).Include(f => f.Venue);
            return View(await booking_facilitiesContext.ToListAsync());
        }

        // GET: Facilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _context.Facility
                .Include(f => f.Sport)
                .Include(f => f.Venue)
                .FirstOrDefaultAsync(m => m.FacilityId == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // GET: Facilities/Create
        public IActionResult Create()
        {
            ViewData["SportId"] = new SelectList(_context.Sport, "SportId", "SportName");
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName");
            return View();
        }

        // POST: Facilities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacilityId,FacilityName,VenueId,SportId")] Facility facility)
        {
            if (_context.Facility.Where(f => f.SportId.Equals(facility.SportId) && f.VenueId.Equals(facility.VenueId)).Any(f => f.FacilityName == facility.FacilityName))
            {
                ModelState.AddModelError("FacilityName", "Facility for this sport already exists at this venue. Please enter another facility name.");
            }
            else if(ModelState.IsValid)
            {
                _context.Add(facility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SportId"] = new SelectList(_context.Sport, "SportId", "SportName", facility.SportId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", facility.VenueId);
            return View(facility);
        }

        // GET: Facilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _context.Facility.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }
            ViewData["SportId"] = new SelectList(_context.Sport, "SportId", "SportName", facility.SportId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", facility.VenueId);
            return View(facility);
        }

        // POST: Facilities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacilityId,FacilityName,VenueId,SportId")] Facility facility)
        {
            if (id != facility.FacilityId)
            {
                return NotFound();
            }

            if (_context.Facility.Where(f => f.SportId.Equals(facility.SportId) && f.VenueId.Equals(facility.VenueId)).Any(f => f.FacilityName == facility.FacilityName))
            {
                ModelState.AddModelError("FacilityName", "Facility for this sport already exists at this venue. Please enter another facility name.");
            }
            else if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(facility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(facility.FacilityId))
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
            ViewData["SportId"] = new SelectList(_context.Sport, "SportId", "SportName", facility.SportId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueId", "VenueName", facility.VenueId);
            return View(facility);
        }

        // GET: Facilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _context.Facility
                .Include(f => f.Sport)
                .Include(f => f.Venue)
                .FirstOrDefaultAsync(m => m.FacilityId == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // POST: Facilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facility = await _context.Facility.FindAsync(id);
            _context.Facility.Remove(facility);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
            return _context.Facility.Any(e => e.FacilityId == id);
        }
    }
}
