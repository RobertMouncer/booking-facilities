using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Models
{
    public class Facility
    {
        public virtual int FacilityId { get; set; }
        public virtual String FacilityName { get; set; }

        public virtual int VenueId { get; set; }
        public virtual Venue Venue { get; set; }

        public virtual int SportId { get; set; }
        public virtual Sport Sport { get; set; }
    }
}
