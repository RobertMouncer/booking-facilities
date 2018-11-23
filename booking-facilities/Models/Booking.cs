using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Models
{
    public class Booking
    {
        [Required]
        public virtual int BookingId { get; set; }
        [Required]
        public virtual int FacilityId { get; set; }
        public virtual Facility Facility { get; set; }
        [Required]
        [DisplayName("Booking Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Column(TypeName = "date")]
        public virtual DateTime BookingDate { get; set; }
        [Required]
        [DisplayName("Booking Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm}")]
        public virtual DateTime BookingTime { get; set; }
        [Required]
        public virtual int UserId { get; set; }
    }
}
