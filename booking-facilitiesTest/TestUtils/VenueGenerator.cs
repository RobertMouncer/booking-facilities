using System;
using System.Collections.Generic;
using System.Text;
using booking_facilities.Models;

namespace booking_facilitiesTest.TestUtils
{
    class VenueGenerator
    {
        public static Venue Create(int index = 0)
        {
            return new Venue
            {
                VenueId = index,
                VenueName = "Old Sports Hall" + index
            };
        }

        public static List<Venue> CreateList(int length = 5)
        {
            List<Venue> list = new List<Venue>();
            for (var i = 0; i < length; i++)
            {
                list.Add(Create(i));
            }
            return list;
        }
    }
}
