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
    public class FacilityAPIController_Test
    {
        private readonly FacilityAPIController controller;
        private readonly Mock<IFacilityRepository> facilityRepository;

        public FacilityAPIController_Test()
        {
            facilityRepository = new Mock<IFacilityRepository>();
            controller = new FacilityAPIController(facilityRepository.Object);
        }

        [Fact]
        public void GetFacilities()
        {
            // implement later
            var facilities = FacilityGenerator.CreateList(5);
            var expected = facilities[2];
            var expectedo = facilities[3];
            var result = controller.GetFacility();
            var ok = result as OkObjectResult;
            var model = ok.Value as Facility;
            Assert.Equal(expected, model);
        }

        [Fact]
        public void GetFacilitiesByVenueSport()
        {
            // implement later
            var facilities = FacilityGenerator.CreateList(5);
            int sportIndex = 3;
            int venueIndex = 3;
            var expected = facilities[sportIndex];
            var result = controller.GetFacilitiesByVenueSport(venueIndex, sportIndex);
            var ok = result as OkObjectResult;
            var model = ok.Value as Facility;
            Assert.Equal(expected, model);
        }

    }
}
