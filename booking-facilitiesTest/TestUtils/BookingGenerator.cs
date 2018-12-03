using System;
using System.Collections.Generic;
using System.Text;
using booking_facilities.Models;

namespace booking_facilitiesTest.TestUtils
{
    class BookingGenerator
    {
        public static Booking Create(int index = 0)
        {
            return new Booking
            {
                BookingId = index,
                FacilityId = FacilityGenerator.Create().FacilityId,
                BookingDateTime = DateTime.Now.AddHours(index),
                EndBookingDateTime = DateTime.Now.AddHours(index+1),
                UserId = "f94e903b-54ea-47c3-b709-ad6d95ec3556",
                IsBlock = false
            };
        }

        public static List<Booking> CreateList(int length = 5)
        {
            List<Booking> list = new List<Booking>();
            for (var i = 0; i < length; i++)
            {
                list.Add(Create(i));
            }
            return list;
        }
    }
}
