using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Models
{
    public class Facility
    {
        [Required]
        public virtual int FacilityId { get; set; }
        [Required]
        public virtual String FacilityName { get; set; }
        [Required]
        public virtual int VenueId { get; set; }
        public virtual Venue Venue { get; set; }
        [Required]
        public virtual int SportId { get; set; }
        public virtual Sport Sport { get; set; }
    }
}
