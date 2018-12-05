using System;
using System.Collections.Generic;
using System.Text;
using booking_facilities.Models;

namespace booking_facilitiesTest.TestUtils
{
    class FacilityGenerator
    {
        public static Facility Create(int index = 0)
        {
            var sport = SportGenerator.Create();
            var venue = VenueGenerator.Create();
            return new Facility
            {
                FacilityName = "Court" + index,
                SportId = sport.SportId,
                Sport = sport,
                Venue = venue,
                VenueId = venue.VenueId
            };
        }

        public static List<Facility> CreateList(int length = 5)
        {
            List<Facility> list = new List<Facility>();
            for (var i = 0; i < length; i++)
            {
                list.Add(Create(i));
            }
            return list;
        }
    }
}
