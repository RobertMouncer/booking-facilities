﻿using System;
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
        [DisplayName("Booking Date & Time")]
        public virtual DateTime BookingDateTime { get; set; }
        [DisplayName("End Booking Date & Time")]
        public virtual DateTime EndBookingDateTime { get; set; }
        [DisplayName("User Email")]
        public virtual String UserId { get; set; }
        [DisplayName("Block/Booking")]
        public virtual bool IsBlock { get; set; }
    }
}
