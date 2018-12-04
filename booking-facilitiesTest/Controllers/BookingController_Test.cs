using booking_facilities.Controllers;
using booking_facilities.Models;
using booking_facilities.Repositories;
using booking_facilities.Services;
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
    public class BookingController_Test
    {

        private readonly Mock<IBookingRepository> bookingRepository;
        private readonly Mock<IFacilityRepository> facilityRepository;
        private readonly Mock<IVenueRepository> venueRepository;
        private readonly Mock<ISportRepository> sportRepository;
        private readonly BookingsController Controller;
        private readonly ApiClient apiClient;

        public BookingController_Test()
        {
            bookingRepository = new Mock<IBookingRepository>();
            facilityRepository = new Mock<IFacilityRepository>();
            venueRepository = new Mock<IVenueRepository>();
            sportRepository = new Mock<ISportRepository>();
            Controller = new BookingsController(facilityRepository.Object, venueRepository.Object, sportRepository.Object, bookingRepository.Object, apiClient);
        }

        //pagination on the index page makes this method difficult
        //also the use of IQueryables which is questionable design
        [Fact]
        public void Index_ShowCorrectView()
        {
            var result = Controller.Index(null, "Date");
            //implement
            Assert.Null(1);
        }

        //pagination on the index page makes this method difficult
        //also the use of IQueryables which is questionable design
        [Fact]
        public void Index_ContainsCorrectModel()
        {
            //implement
            var expectedResources = BookingGenerator.CreateList();
            Assert.Equal(1, 2);
        }

        [Fact]
        public void Create_ShowsCorrectView()
        {
            var result = Controller.Create();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        public void Createblock_ShowsCorrectView()
        {
            var result = Controller.CreateBlockFacility();
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void Create_ContainsCorrectModel()
        {
            //implement
            Assert.Equal(1,2);
        }
        [Fact]

        public void CreateBlock_ContainsCorrectModel()
        {
            //implement
            Assert.Null(1);
        }

        [Fact]
        public void Delete_ShowsCorrectView()
        {
            bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BookingGenerator.Create());
            //implement
            Assert.Null(1);
        }

        [Fact]
        public void Delete_ContainsCorrectModel()
        {
            //implement
            Assert.Equal(1, 2);
        }

        [Fact]
        public void Details_ShowsCorrectView()
        {
            //implement
            Assert.Equal(1, 2);
        }

        [Fact]
        public async void Details_ContainsCorrectModel()
        {
            //implement
            Assert.Equal(1,2);
        }

        [Fact]
        public async void DeleteConfirmed_DeleteBooking()
        {
            var booking = BookingGenerator.Create();

            var result = await Controller.DeleteConfirmed(booking.BookingId);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectedResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectedResult.ActionName);

            bookingRepository.Verify();
        }

        [Fact]
        public async void Update_ShowsCorrectView()
        {
            bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BookingGenerator.Create());
            var result = await Controller.Edit(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void Update_ContainsCorrectModel()
        {
            var expectedResource = BookingGenerator.Create();
            bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.Edit(1) as ViewResult;
            Assert.IsType<Booking>(viewResult.Model);

            var resources = viewResult.Model as Booking;
            Assert.Equal(expectedResource, resources);
        }

        [Fact]
        public async void UpdateBlock_ShowsCorrectView()
        {
            bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BookingGenerator.CreateBlock());
            var result = await Controller.EditBlockFacility(1);
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async void UpdateBlock_ContainsCorrectModel()
        {
            var expectedResource = BookingGenerator.CreateBlock();
            bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedResource);

            var viewResult = await Controller.EditBlockFacility(1) as ViewResult;
            Assert.IsType<Booking>(viewResult.Model);

            var resources = viewResult.Model as Booking;
            Assert.Equal(expectedResource, resources);
        }
    }
}
