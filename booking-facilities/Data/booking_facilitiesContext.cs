using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using booking_facilities.Models;

namespace booking_facilities.Models
{
    public class booking_facilitiesContext : DbContext
    {
        public booking_facilitiesContext(DbContextOptions<booking_facilitiesContext> options)
            : base(options)
        {
        }

        public DbSet<booking_facilities.Models.Venue> Venue { get; set; }

        public DbSet<booking_facilities.Models.Sport> Sport { get; set; }

        public DbSet<booking_facilities.Models.Facility> Facility { get; set; }

        public DbSet<booking_facilities.Models.Booking> Booking { get; set; }

    }
}
