using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using booking_facilities.Repositories;

namespace booking_facilities.Controllers
{
    [Authorize(AuthenticationSchemes = "oidc", Policy = "administrator")]
    public class FacilitiesController : Controller
    {
        private readonly IFacilityRepository facilityRepository;
        private readonly IVenueRepository venueRepository;
        private readonly ISportRepository sportRepository;
        private readonly IBookingRepository bookingRepository;

        public FacilitiesController(IFacilityRepository facilityRepository,
                                    IVenueRepository venueRepository,
                                    ISportRepository sportRepository,
                                    IBookingRepository bookingRepository)
        {
            this.facilityRepository = facilityRepository;
            this.venueRepository = venueRepository;
            this.sportRepository = sportRepository;
            this.bookingRepository = bookingRepository;
        }

        // GET: Facilities
        public async Task<IActionResult> Index(int? page, string sortOrder)
        {
            ViewData["VenueSortParm"] = sortOrder == "Venue" ? "venue_desc" : "Venue";
            ViewData["SportSortParm"] = sortOrder == "Sport" ? "sport_desc" : "Sport";

            IQueryable<Facility> facilities = facilityRepository.GetAllAsync().Include(f => f.Sport).Include(f => f.Venue);

            switch (sortOrder)
            {
                case "venue_desc":
                    facilities = facilities.OrderByDescending(f => f.Venue.VenueName);
                    break;
                case "Sport":
                    facilities = facilities.OrderBy(f => f.Sport.SportName);
                    break;
                case "sport_desc":
                    facilities = facilities.OrderByDescending(f => f.Sport.SportName);
                    break;
                default:
                    facilities = facilities.OrderBy(f => f.Venue.VenueName);
                    break;
            }

            var facilitiesList = await facilities.ToListAsync();

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var facilitiesPerPage = 10;

            var onePageOfFacilities = facilitiesList.ToPagedList(pageNumber, facilitiesPerPage); // will only contain 25 products max because of the pageSize

            ViewBag.onePageOfFacilities = onePageOfFacilities;
            return View();

        }

        // GET: Facilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await facilityRepository.GetAllAsync()
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
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            return View();
        }

        // POST: Facilities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacilityId,FacilityName,VenueId,SportId")] Facility facility)
        {
            if (facilityRepository.GetAllAsync().Where(f => f.SportId.Equals(facility.SportId) && f.VenueId.Equals(facility.VenueId)).Any(f => f.FacilityName == facility.FacilityName))
            {
                ModelState.AddModelError("FacilityName", "Facility for this sport already exists at this venue. Please enter another facility name.");
            }
            else if (ModelState.IsValid)
            {
                await facilityRepository.AddAsync(facility);
                return RedirectToAction(nameof(Index));
            }
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName", facility.SportId);
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName", facility.VenueId);
            return View(facility);
        }

        // GET: Facilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await facilityRepository.GetByIdAsync(id.Value);
            if (facility == null)
            {
                return NotFound();
            }
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName", facility.SportId);
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName", facility.VenueId);
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

            if (facilityRepository.GetAllAsync().Where(f => f.SportId.Equals(facility.SportId) && f.VenueId.Equals(facility.VenueId)).Any(f => f.FacilityName == facility.FacilityName))
            {
                ModelState.AddModelError("FacilityName", "Facility for this sport already exists at this venue. Please enter another facility name.");
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    //var bookings = _context.Booking.Where(b => b.FacilityId.Equals(facility.FacilityId) && !b.IsBlock);
                    var bookings = bookingRepository.GetAllAsync();
                    var bookingsForFacility = bookingRepository.GetAllAsync().Where(b => b.FacilityId.Equals(facility.FacilityId) && !b.IsBlock);
                    bookings.RemoveRange(bookingsForFacility);
                    await facilityRepository.UpdateAsync(facility);
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
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName", facility.SportId);
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName", facility.VenueId);
            return View(facility);
        }

        // GET: Facilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await facilityRepository.GetAllAsync()
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
            var facility = await facilityRepository.GetByIdAsync(id);
            await facilityRepository.DeleteAsync(facility);
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
            return facilityRepository.FacilityExists(id);
        }
    }
}
