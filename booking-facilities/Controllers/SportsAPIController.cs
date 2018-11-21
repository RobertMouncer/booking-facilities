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
    [Route("Sports")]
    [ApiController]
    public class SportsAPIController : ControllerBase
    {
        private readonly booking_facilitiesContext _context;

        public SportsAPIController(booking_facilitiesContext context)
        {
            _context = context;
        }

        // GET: api/SportsAPI
        //[HttpGet]
        //public IEnumerable<Sport> GetSport()
        //{
        //    return _context.Sport;
        //}

        // GET: api/SportsAPI/5
        [HttpGet("getSportsByVenue/{id}")]
        public async Task<IActionResult> GetSport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var facilities = _context.Facility.Where(f => f.VenueId.Equals(id));
            var sports = _context.Sport;
            var results = new List<Sport>();
            foreach (Facility f in facilities)
            {
                results.Add(sports.Where(s => s.SportId.Equals(f.SportId)).Single()); 
            }

            if (facilities == null)
            {
                return NotFound();
            }

            return Ok(results);
        }

    }
}