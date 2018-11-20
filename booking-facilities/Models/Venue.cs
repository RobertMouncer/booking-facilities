using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Models
{
    public class Venue
    {
        [Required]
        public virtual int VenueId { get; set; }
        [Required]
        public virtual String VenueName { get; set; }
    }
}
