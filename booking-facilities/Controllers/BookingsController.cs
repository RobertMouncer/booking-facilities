using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using booking_facilities.Services;
using Newtonsoft.Json.Linq;
using X.PagedList;
using booking_facilities.Repositories;
namespace booking_facilities.Controllers
{
    [Authorize(AuthenticationSchemes = "oidc")]
    public class BookingsController : Controller
    {
        private readonly IFacilityRepository facilityRepository;
        private readonly IVenueRepository venueRepository;
        private readonly ISportRepository sportRepository;
        private readonly IBookingRepository bookingRepository;
        private readonly IApiClient apiClient;

        public BookingsController(IFacilityRepository facilityRepository, IVenueRepository venueRepository, ISportRepository sportRepository, IBookingRepository bookingRepository, IApiClient client)
        {
            this.venueRepository = venueRepository;
            this.facilityRepository = facilityRepository;
            this.sportRepository = sportRepository;
            this.bookingRepository = bookingRepository;
            apiClient = client;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(int? page, string sortOrder)
        {
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["VenueSortParm"] = sortOrder == "Venue" ? "venue_desc" : "Venue";
            ViewData["UserSortParm"] = sortOrder == "User" ? "user_desc" : "User";
            IQueryable<Booking> booking_facilitiesContext = bookingRepository.GetAllAsync().Include(b => b.Facility)
                                                                                      .Include(b => b.Facility.Venue)
                                                                                      .Include(b => b.Facility.Sport)
                                                                                      .Where(b => (DateTime.Compare(b.EndBookingDateTime, DateTime.Now) >= 0));

            if (!User.Claims.FirstOrDefault(c => c.Type == "user_type").Value.Equals("administrator"))
            {
                booking_facilitiesContext = booking_facilitiesContext.Where(b => b.UserId.Equals(User.Claims.FirstOrDefault(c => c.Type == "sub").Value) || b.IsBlock);
            }
            switch (sortOrder)
            {
                case "date_desc":
                    booking_facilitiesContext = booking_facilitiesContext.OrderByDescending(b => b.BookingDateTime);
                    break;
                case "Venue":
                    booking_facilitiesContext = booking_facilitiesContext.OrderBy(b => b.Facility.Venue);
                    break;
                case "venue_desc":
                    booking_facilitiesContext = booking_facilitiesContext.OrderByDescending(b => b.Facility.Venue);
                    break;
                case "User":
                    booking_facilitiesContext = booking_facilitiesContext.OrderBy(b => b.UserId);
                    break;
                case "user_desc":
                    booking_facilitiesContext = booking_facilitiesContext.OrderByDescending(b => b.UserId);
                    break;
                default:
                    booking_facilitiesContext = booking_facilitiesContext.OrderBy(b => b.BookingDateTime);
                    break;

            }

            var bookingList = await booking_facilitiesContext.ToListAsync();

            List<string> userList = new List<string>();
            foreach(Booking b in bookingList)
            {
                userList.Add(b.UserId);
            }
            
            var response = await apiClient.PostAsync("https://docker2.aberfitness.biz/gatekeeper/api/Users/Batch", userList.Distinct());
            JArray jsonArrayOfUsers = JArray.Parse(await response.Content.ReadAsStringAsync());
            foreach (Booking b in bookingList)
            {
                foreach (JObject j in jsonArrayOfUsers)
                {
                    if (b.UserId == j.GetValue("id").ToString())
                    {
                        b.UserId = j.GetValue("email").ToString();
                    }
                }
            }

            
            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var bookingsPerPage = 10;
            var onePageOfBookings = bookingList.ToPagedList(pageNumber, bookingsPerPage); // will only contain 25 products max because of the pageSize

            ViewBag.onePageOfBookings = onePageOfBookings;
            return View();

        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var booking = await bookingRepository.GetByIdAndInclude(id.Value);
            var booking = await bookingRepository.GetAllAsync().Include(b => b.Facility)
               .Include(b => b.Facility.Venue)
               .Include(b => b.Facility.Sport)
               .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            var response = await apiClient.GetAsync("https://docker2.aberfitness.biz/gatekeeper/api/Users/" + booking.UserId);
            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);
            booking.UserId = data.email;

            return View(booking);
        }

        // GET: Bookings/CreateBlockFacility
        [Authorize(Policy = "Administrator")]
        public IActionResult CreateBlockFacility()
        {
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["FacilityId"] = new SelectList(facilityRepository.GetAllAsync(), "FacilityId", "FacilityName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> CreateBlockFacility([Bind("BookingId,FacilityId,BookingDateTime,UserId,EndBookingDateTime")] Booking booking, [Bind("VenueId")] int VenueId, [Bind("SportId")] int SportId)
        {
            booking.IsBlock = true;

            booking.UserId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var bookings = bookingRepository.GetAllAsync().Where(b => b.FacilityId.Equals(booking.FacilityId));

            if (DateTime.Compare(booking.BookingDateTime, DateTime.Now) <= 0)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is in the past. Please enter future Date/Time.");
            }
            if (DateTime.Compare(booking.EndBookingDateTime, booking.BookingDateTime) <= 0)
            {
                ModelState.AddModelError("EndBookingDateTime", "End Date/Time should be after the start Date/Time. Please re-enter Date/Time.");
            }
            if (ModelState.IsValid)
            {

                foreach(Booking b in bookings)
                {
                    //true if (new booking start time is before old booking start time) AND if (new booking end time is after old booking end time)
                    if (DateTime.Compare(booking.BookingDateTime,b.BookingDateTime) <= 0 && DateTime.Compare(b.BookingDateTime, booking.EndBookingDateTime.AddHours(-1)) <= 0 && !b.IsBlock)
                    {
                        await bookingRepository.DeleteAsync(b);
                    }
                }

                await bookingRepository.AddAsync(booking);

                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["FacilityId"] = new SelectList(facilityRepository.GetAllAsync(), "FacilityId", "FacilityName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName"); 
            return View(booking);
        }

        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> EditBlockFacility(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await bookingRepository.GetByIdAsync(id.Value);

            if (booking == null)
            {
                return NotFound();
            }
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["FacilityId"] = new SelectList(facilityRepository.GetAllAsync(), "FacilityId", "FacilityName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrator")]
        public async Task<IActionResult> EditBlockFacility(int id, [Bind("BookingId,FacilityId,BookingDateTime,UserId,EndBookingDateTime")] Booking booking, [Bind("VenueId")] int VenueId, [Bind("SportId")] int SportId)
        {

            //implement edit
            //do times
            booking.IsBlock = true;

            booking.UserId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            //must compare bookingId in where because you can't inspect a booking currently being edited WTF!!!!

            var bookings = bookingRepository.GetAllAsync().Where(b => b.FacilityId.Equals(booking.FacilityId)
            && !b.BookingId.Equals(booking.BookingId));

            if (DateTime.Compare(booking.BookingDateTime, DateTime.Now) <= 0)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is in the past. Please enter future Date/Time.");
            }
            if (DateTime.Compare(booking.EndBookingDateTime, booking.BookingDateTime) <= 0)
            {
                ModelState.AddModelError("EndBookingDateTime", "End Date/Time should be after the start Date/Time. Please re-enter Date/Time.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (Booking b in bookings)
                    {
                        if (DateTime.Compare(booking.BookingDateTime, b.BookingDateTime) <= 0 && DateTime.Compare(b.BookingDateTime, booking.EndBookingDateTime.AddHours(-1)) <= 0 && !b.IsBlock)
                        {
                            
                            await bookingRepository.DeleteAsync(b);
                        }
                    }
                       await bookingRepository.UpdateAsync(booking);
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
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["FacilityId"] = new SelectList(facilityRepository.GetAllAsync(), "FacilityId", "FacilityName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            return View(booking);
        }

            // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            return View();
        }
        


        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,FacilityId,BookingDateTime,UserId,EndBookingDateTime")] Booking booking, [Bind("VenueId")] int VenueId, [Bind("SportId")] int SportId)
        {
            
            booking.EndBookingDateTime = booking.BookingDateTime.AddHours(1);
            booking.UserId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            var bookings = bookingRepository.GetBookingsInLocationAtDateTime(booking.BookingDateTime, VenueId, SportId);
            var facilities = facilityRepository.GetAllAsync();
            var faciltiesFiltered = facilities.Where(f => f.VenueId.Equals(VenueId) && f.SportId.Equals(SportId));

            booking.FacilityId = getFacility(VenueId, SportId, booking);

            //adds model errors if date input is not correct
            //checks date/time to be booked is after current date/time -> else add model error.
            //and adds model error if facilities are not available. 
            if (DateTime.Compare(booking.BookingDateTime, DateTime.Now) <= 0)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is in the past. Please enter future Date/Time.");
            }

            if (booking.FacilityId == -1)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is no longer available. Please try again.");
            }

            if (ModelState.IsValid)
            {
                await bookingRepository.AddAsync(booking);
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await bookingRepository.GetByIdAsync(id.Value);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,FacilityId,BookingDateTime,UserId,EndBookingDateTime")] Booking booking, [Bind("VenueId")] int VenueId, [Bind("SportId")] int SportId)
        {
            booking.EndBookingDateTime = booking.BookingDateTime.AddHours(1);
            booking.UserId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            var bookings = bookingRepository.GetBookingsInLocationAtDateTime(booking.BookingDateTime, VenueId, SportId);
            var facilities = facilityRepository.GetAllAsync();


            var faciltiesFiltered = facilities.Where(f => f.VenueId.Equals(VenueId) && f.SportId.Equals(SportId));

            booking.FacilityId = getFacility(VenueId, SportId, booking);

            if (DateTime.Compare(booking.BookingDateTime, DateTime.Now) <= 0)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is in the past. Please enter future Date/Time.");
            }
            if (booking.FacilityId == -1)
            {
                ModelState.AddModelError("BookingDateTime", "Date/Time is no longer available. Please try again.");
            }
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    await bookingRepository.UpdateAsync(booking);
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
            ViewData["VenueId"] = new SelectList(venueRepository.GetAllAsync(), "VenueId", "VenueName");
            ViewData["SportId"] = new SelectList(sportRepository.GetAllAsync(), "SportId", "SportName");
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            

            var booking = await bookingRepository.GetAllAsync().Include(b => b.Facility)
                                                               .Include(b => b.Facility.Venue)
                                                               .Include(b => b.Facility.Sport)
                                                               .FirstOrDefaultAsync(m => m.BookingId == id);

            if (User.Claims.FirstOrDefault(c => c.Type == "sub").Value != booking.UserId 
                && !User.Claims.FirstOrDefault(c => c.Type == "user_type").Value.Equals("administrator"))
            {
                return Unauthorized();
            }


            if (booking == null)
            {
                return NotFound();
            }
            
            var response = await apiClient.GetAsync("https://docker2.aberfitness.biz/gatekeeper/api/Users/" + booking.UserId);
            var json = await response.Content.ReadAsStringAsync();
            dynamic data = JObject.Parse(json);
            booking.UserId = data.email;

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await bookingRepository.GetByIdAsync(id);
            await bookingRepository.DeleteAsync(booking);
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return bookingRepository.GetAllAsync().Any(e => e.BookingId == id);
        }

        private int getFacility(int VenueId, int SportId, Booking booking)
        {
            var facilities = facilityRepository.GetAllAsync().Where(f => f.VenueId.Equals(VenueId) && f.SportId.Equals(SportId));


            //var bookings = _context.Booking.Where(b => b.BookingDateTime.Equals(booking.BookingDateTime)
            //&& b.Facility.VenueId.Equals(VenueId)
            //&& b.Facility.SportId.Equals(SportId)
            //&& !b.IsBlock);
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
