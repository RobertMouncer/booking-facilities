using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [MaxLength(50)]
        [DisplayName("Facility Name")]
        public virtual String FacilityName { get; set; }
        [Required]
        [DisplayName("Venue")]
        public virtual int VenueId { get; set; }
        public virtual Venue Venue { get; set; }
        [Required]
        [DisplayName("Sport")]
        public virtual int SportId { get; set; }
        public virtual Sport Sport { get; set; }
    }
}
