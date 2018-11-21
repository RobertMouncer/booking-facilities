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
    [Route("/getFacilities")]
    [ApiController]
    public class FacilityAPIController : ControllerBase
    {
        private readonly booking_facilitiesContext _context;

        public FacilityAPIController(booking_facilitiesContext context)
        {
            _context = context;
        }

        // GET: api/FacilityAPI
        [HttpGet]
        public IEnumerable<Facility> GetFacility()
        {
            return _context.Facility;
        }

    }
}