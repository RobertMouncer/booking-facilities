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
            return new Facility
            {
                FacilityId = index,
                FacilityName = "Court" + index,
                SportId = SportGenerator.Create().SportId,
                VenueId = VenueGenerator.Create().VenueId
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
