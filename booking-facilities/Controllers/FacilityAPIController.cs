using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;
using Microsoft.AspNetCore.Authorization;

namespace booking_facilities.Controllers
{
    [Authorize(Policy = "APIPolicy")]
    [Route("api/getFacilities")]
    [ApiController]
    public class FacilityAPIController : ControllerBase
    {
        private readonly booking_facilitiesContext _context;

        public FacilityAPIController(booking_facilitiesContext context)
        {
            _context = context;
        }

        // GET: api/getFacilities
        [HttpGet]
        public IEnumerable<Facility> GetFacility()
        {
            return _context.Facility;
        }

    }
}