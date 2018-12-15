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
using AberFitnessAuditLogger;

namespace booking_facilities.Controllers
{
    [Authorize(AuthenticationSchemes = "oidc", Policy = "administrator")]
    public class FacilitiesController : Controller
    {
        private readonly IFacilityRepository facilityRepository;
        private readonly IVenueRepository venueRepository;
        private readonly ISportRepository sportRepository;
        private readonly IBookingRepository bookingRepository;
        private readonly IAuditLogger auditLogger;

        public FacilitiesController(IFacilityRepository facilityRepository,
                                    IVenueRepository venueRepository,
                                    ISportRepository sportRepository,
                                    IBookingRepository bookingRepository,
                                    IAuditLogger auditLogger)
        {
            this.facilityRepository = facilityRepository;
            this.venueRepository = venueRepository;
            this.sportRepository = sportRepository;
            this.bookingRepository = bookingRepository;
            this.auditLogger = auditLogger;
        }

        // GET: Facilities
        public async Task<IActionResult> Index(int? page, string sortOrder)
        {
            await auditLogger.log(GetUserId(), "Accessed Index Facilities");
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

        // GET: Facilities/Create
        public async Task<IActionResult> Create()
        {
            await auditLogger.log(GetUserId(), "Accessed Create Facilities");
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
                facility = await facilityRepository.AddAsync(facility);
                await auditLogger.log(GetUserId(), $"Created facility {facility.FacilityId}");
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
            await auditLogger.log(GetUserId(), $"Accessed Edit Facility {id}");
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
                    var bookingsForFacility = bookingRepository.GetAllAsync().Where(b => b.FacilityId.Equals(facility.FacilityId) && !b.IsBlock);
                    foreach(Booking b in bookingsForFacility)
                    {
                        await bookingRepository.DeleteAsync(b);
                        await auditLogger.log(GetUserId(), $"Deleted bookings {b.BookingId}");
                    }

                    facility = await facilityRepository.UpdateAsync(facility);
                    await auditLogger.log(GetUserId(), $"Updated facility {facility.FacilityId}");
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

            await auditLogger.log(GetUserId(), $"Accessed Delete facility {id}");

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
            await auditLogger.log(GetUserId(), $"Deleted facility {id}");
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
            return facilityRepository.FacilityExists(id);
        }
        public string GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
        }
    }
}
