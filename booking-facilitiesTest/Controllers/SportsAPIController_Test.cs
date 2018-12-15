using booking_facilities.Controllers;
using booking_facilities.Models;
using booking_facilities.Repositories;
using booking_facilitiesTest.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace booking_facilitiesTest.Controllers
{
    public class SportsAPIController_Test
    {
        private readonly SportsAPIController controller;
        private readonly Mock<ISportRepository> sportRepository;
        private readonly Mock<IFacilityRepository> facilityRepository;

        public SportsAPIController_Test()
        {
            sportRepository = new Mock<ISportRepository>();
            facilityRepository = new Mock<IFacilityRepository>();
            controller = new SportsAPIController(sportRepository.Object, facilityRepository.Object);
        }

        [Fact]
        public async void GetSportsFromVenueAsync()
        {
            var facilities = FacilityGenerator.CreateList(5);
            var expected = facilities[2].Sport;

            foreach (Facility f in facilities)
            {
                await facilityRepository.Object.AddAsync(f);

            }

            var result = controller.GetSport(facilities[2].VenueId);
            var ok = result as OkObjectResult;
            var model = ok.Value as Sport;


            Assert.Equal(expected,model);
        }

    }
}
