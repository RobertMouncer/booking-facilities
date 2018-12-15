using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace booking_facilities.Controllers
{
    [Route("api/status")]
    [ApiController]
    public class StatusAPIController : ControllerBase
    {
        // GET: /api/status
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
