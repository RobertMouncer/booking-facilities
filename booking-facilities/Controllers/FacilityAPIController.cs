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

    [Route("api/Facilities")]
    [ApiController]
    public class FacilityAPIController : ControllerBase
    {
        private readonly IFacilityRepository facilityRepository;

        public FacilityAPIController(IFacilityRepository facilityRepository)
        {
            this.facilityRepository = facilityRepository;
        }

        // GET: api/getFacilities
        [HttpGet]
        public IEnumerable<Facility> GetFacility()
        {
            return facilityRepository.GetAllAsync();
        }

        [HttpGet("{venueId}/{sportId}")]
        public IActionResult GetFacilitiesByVenueSport([FromRoute] int venueId, [FromRoute] int sportId)
        {
            var facilities = facilityRepository.GetAllAsync().Where(f => f.VenueId.Equals(venueId) && f.SportId.Equals(sportId));

            return Ok(facilities.ToList());
        }

    }
}