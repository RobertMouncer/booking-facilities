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
    [Route("api/venues")]
    [ApiController]
    public class VenueAPIController : ControllerBase
    {
        private readonly IVenueRepository venueRepository;

        public VenueAPIController(IVenueRepository venueRepository)
        {
            this.venueRepository = venueRepository;
        }

        // GET: venues
        [HttpGet]
        public IActionResult GetAllVenues()
        {
            var venues = venueRepository.GetAllAsync();
            var results = new List<Venue>();
            return Ok(results);
        }
    }
}