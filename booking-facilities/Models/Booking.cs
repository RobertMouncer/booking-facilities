using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Models
{
    public class Booking
    {

        public virtual int BookingId { get; set; }

        public virtual int FacilityId { get; set; }
        public virtual Facility Facility { get; set; }

        public virtual DateTime BookingDateTime { get; set; }

        public virtual int UserId { get; set; }
    }
}
