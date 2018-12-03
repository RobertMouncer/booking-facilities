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
    [Route("api/sports")]
    [ApiController]
    public class SportsAPIController : ControllerBase
    {
        private readonly ISportRepository sportRepository;
        private readonly IFacilityRepository facilityRepository;

        public SportsAPIController(ISportRepository sportRepository, IFacilityRepository facilityRepository)
        {
            this.sportRepository = sportRepository;
            this.facilityRepository = facilityRepository;
        }


        // GET: sports/getSportsByVenue/5
        [HttpGet("getSportsByVenue/{id}")]
        public IActionResult GetSport([FromRoute] int id)
        {

            var facilities = facilityRepository.GetFacilityByVenueAsync(id);
            var sports = sportRepository.GetAllAsync();
            var results = new List<Sport>();
            foreach (Facility f in facilities)
            {
                results.Add(sports.Where(s => s.SportId.Equals(f.SportId)).Single());
            }

            return Ok(results.Distinct());
        }

    }
}