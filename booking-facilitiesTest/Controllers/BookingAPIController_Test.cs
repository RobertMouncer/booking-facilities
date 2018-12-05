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
    public class BookingAPIController_Test
    {
        private readonly BookingAPIController controller;
        private readonly Mock<IBookingRepository> bookingRepository;
        private readonly Mock<IFacilityRepository> facilityRepository;

        public BookingAPIController_Test()
        {
            bookingRepository = new Mock<IBookingRepository>();
            facilityRepository = new Mock<IFacilityRepository>();
            controller = new BookingAPIController( facilityRepository.Object, bookingRepository.Object);
        }

        [Fact]
        public void GetBooking_ReturnsAllBookings()
        {
            var bookings = BookingGenerator.CreateList(5);
            bookingRepository.Setup(r => r.GetAllAsync());
            var result = controller.GetBooking();
            //implement
            Assert.Equal(1,2);
        }

        [Fact]
        public async void GetBooking_ReturnsNotFoundOnInvalidId()
        {
            bookingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Booking)null).Verifiable();
            var result = await controller.GetBooking(1);
            Assert.IsType<NotFoundResult>(result);
            bookingRepository.Verify();
            bookingRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetBooking_ReturnsBooking()
        {
            var booking = BookingGenerator.Create();
            bookingRepository.Setup(r => r.GetByIdAsync(booking.BookingId)).ReturnsAsync(booking).Verifiable();
            var result = await controller.GetBooking(booking.BookingId);
            Assert.IsType<OkObjectResult>(result);
            var content = result as OkObjectResult;
            Assert.IsType<Booking>(content.Value);
            Assert.Equal(booking, content.Value);
            bookingRepository.Verify();
            bookingRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async void PutBooking_ReturnsBadRequest()
        {
            var facility = FacilityGenerator.Create();
            var invalidBooking = new Booking
            {
                BookingId = 1,
                FacilityId = facility.FacilityId,
                BookingDateTime = new DateTime(2018, 10, 15),
                EndBookingDateTime = new DateTime(2018, 10, 14),
                Facility = facility,
                UserId = "abc123"
                
            };
            bookingRepository.Setup(atr => atr.GetByIdAsync(invalidBooking.BookingId)).ReturnsAsync((Booking)null).Verifiable();

            var result = await controller.PutBooking(0,facility.VenueId,facility.SportId, invalidBooking);

            //implement
            bookingRepository.Verify();
            bookingRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async void PutBooking_Updates()
        {
            var facility = FacilityGenerator.Create();
            var booking = new Booking
            {
                BookingId = 1,
                FacilityId = facility.FacilityId,
                BookingDateTime = new DateTime(2018, 10, 15),
                EndBookingDateTime = new DateTime(2018, 10, 14),
                Facility = facility,
                UserId = "abc123"

            };

            bookingRepository.Setup(b => b.GetByIdAsync(booking.BookingId)).ReturnsAsync(booking).Verifiable();

            var result = await controller.PutBooking(1, facility.VenueId, facility.SportId, booking);
            //implement
            Assert.Equal(1, 2);
            bookingRepository.Verify();
            bookingRepository.VerifyNoOtherCalls();

        }

        [Fact]
        public  void PostBooking_CreatesBooking()
        {
            var booking = BookingGenerator.Create();
            Facility facility = FacilityGenerator.Create();
            booking.FacilityId = facility.FacilityId;
            bookingRepository.Setup(b => b.GetByIdAsync(booking.BookingId)).ReturnsAsync(booking).Verifiable();

            //implement
            Assert.Equal(1, 2);
        }

        [Fact]
        public async void Deletebooking_ReturnsNotFound()
        {
            bookingRepository.Setup(ar => ar.GetByIdAsync(1)).ReturnsAsync((Booking)null).Verifiable();

            var result = await controller.DeleteBooking(1);
            Assert.IsType<NotFoundResult>(result);
            bookingRepository.Verify();
            bookingRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async void DeleteBooking_RemovesBooking()
        {
            var booking = BookingGenerator.Create();
            bookingRepository.Setup(ar => ar.GetByIdAsync(booking.BookingId)).ReturnsAsync(booking).Verifiable();
            bookingRepository.Setup(ar => ar.DeleteAsync(booking)).ReturnsAsync(booking).Verifiable();

            var result = await controller.DeleteBooking(booking.BookingId);
            Assert.IsType<OkObjectResult>(result);
            var content = result as OkObjectResult;
            Assert.IsType<Booking>(content.Value);
            Assert.Equal(booking, content.Value);
            bookingRepository.Verify();
            bookingRepository.VerifyNoOtherCalls();
        }
        [Fact]
        public void getFacilities()
        {
            //implement
            Assert.True(false);
        }

        [Fact]
        public void getTimes()
        {
            //implement
            Assert.True(false);
        }
    }
}
