using AberFitnessAuditLogger;
using booking_facilities.Controllers;
using booking_facilities.Models;
using booking_facilities.Repositories;
using booking_facilitiesTest.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace booking_facilitiesTest.Controllers
{
    public class FacilitiesController_Test
    {

        private readonly Mock<IFacilityRepository> facilityRepository;
        private readonly Mock<IVenueRepository> venueRepository;
        private readonly Mock<ISportRepository> sportRepository;
        private readonly Mock<IBookingRepository> bookingRepository;
        private readonly FacilitiesController Controller;
        private readonly IAuditLogger auditLogger;

        public FacilitiesController_Test()
        {
            facilityRepository = new Mock<IFacilityRepository>();
            venueRepository = new Mock<IVenueRepository>();
            sportRepository = new Mock<ISportRepository>();
            bookingRepository = new Mock<IBookingRepository>();
            Controller = new FacilitiesController(facilityRepository.Object, 
                                                    venueRepository.Object, 
                                                    sportRepository.Object, 
                                                    bookingRepository.Object, auditLogger);
        }

        [Fact]
        public void Index_ShowCorrectView()
        {
            var result = Controller.Index(null, "");
           //implement
            Assert.Null(result);
        }

        [Fact]
        public void Index_ContainsCorrectModel()
        {
            var expectedResources = FacilityGenerator.CreateList();
            //implement
            Assert.Equal(1,2);
        }

        [Fact]
        public void Create_ShowsCorrectView()
        {
            var result = Controller.Create();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void Create_ContainsCorrectModel()
        {
            var expectedResources = VenueGenerator.CreateList();
            //implement
            Assert.Equal(1, 2);
        }

        [Fact]
        public void Delete_ShowsCorrectView()
        {
            var result = Controller.Index(null, "");
            //implement
            Assert.Null(result);
        }

        [Fact]
        public void Delete_ContainsCorrectModel()
        {
            var expectedResources = VenueGenerator.CreateList();
            //implement
            Assert.Equal(1, 2);
        }

        [Fact]
        public void Details_ShowsCorrectView()
        {
            var result = Controller.Index(null, "");
            //implement
            Assert.Null(result);
        }

        [Fact]
        public void Details_ContainsCorrectModel()
        {
            var expectedResources = VenueGenerator.CreateList();
            //implement
            Assert.Equal(1, 2);
        }

        [Fact]
        public async void DeleteConfirmed_DeletesFacility()
        {
            var facility = FacilityGenerator.Create();

            var result = await Controller.DeleteConfirmed(facility.FacilityId);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectedResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectedResult.ActionName);

            facilityRepository.Verify();
        }

        [Fact]
        public async void Update_ShowsCorrectView()
        {
            facilityRepository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(FacilityGenerator.Create());
            var result = await Controller.Edit(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Update_ContainsCorrectModel()
        {
            var expectedResource = FacilityGenerator.Create();
            facilityRepository.Setup(v => v.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Edit(1) as ViewResult;
            Assert.IsType<Facility>(viewResult.Model);

            var resources = viewResult.Model as Facility;
            Assert.Equal(expectedResource, resources);
        }
    }
}
