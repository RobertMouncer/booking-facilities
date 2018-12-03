using System;
using System.Collections.Generic;
using System.Text;
using booking_facilities.Models;

namespace booking_facilitiesTest.TestUtils
{
    class SportGenerator
    {
        public static Sport Create(int index = 0)
        {
            return new Sport
            {
                SportId = index,
                SportName = "Cricket" + index
            };
        }

        public static List<Sport> CreateList(int length = 5)
        {
            List<Sport> list = new List<Sport>();
            for (var i = 0; i < length; i++)
            {
                list.Add(Create(i));
            }
            return list;
        }
    }
}
