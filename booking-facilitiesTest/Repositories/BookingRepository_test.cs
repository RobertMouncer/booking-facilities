using booking_facilities.Models;
using booking_facilitiesTest.TestUtils;
using booking_facilities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace booking_facilitiesTest.Repositories
{
    public class BookingRepository_test
    {
        private static readonly Random random = new Random();
        private readonly DbContextOptions<booking_facilitiesContext> contextOptions;

        public BookingRepository_test()
        {
            contextOptions = new DbContextOptionsBuilder<booking_facilitiesContext>()
                .UseInMemoryDatabase($"rand_db_name_{random.Next()}").Options;
        }

        [Fact]
        public async void GetAllAsync_ReturnsAllFromContext()
        {
            var expectedBookings = BookingGenerator.CreateList();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Booking.AddRange(expectedBookings);
                context.SaveChanges();

                Assert.Equal(expectedBookings.Count, await context.Booking.CountAsync());

                var repository = new BookingRepository(context);
                var resources = repository.GetAllAsync();

                Assert.IsAssignableFrom<IQueryable<Booking>>(resources);
                Assert.Equal(expectedBookings, resources);
            }
        }

        [Fact]
        public async void DeleteAsync_DeleteFromContext()
        {
            var booking = BookingGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                var repository = new BookingRepository(context);
                context.Database.EnsureCreated();
                context.Booking.Add(booking);
                context.SaveChanges();
                Assert.Equal(1, await context.Booking.CountAsync());

                await repository.DeleteAsync(booking);
                Assert.Equal(0, await context.Sport.CountAsync());
            }
        }

        [Fact]
        public async void AddAsync_AddAsyncToConext()
        {
            var booking = BookingGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new BookingRepository(context);
                await repository.AddAsync(booking);
                Assert.Equal(1, await context.Booking.CountAsync());
                Assert.Equal(booking, await context.Booking.SingleAsync());
            }
        }

        [Fact]
        public async void GetByIdAsync_ReturnsCorrectItems()
        {
            var bookings = BookingGenerator.CreateList(5);
            var expected = bookings[2];

            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Booking.AddRange(bookings);

                await context.SaveChangesAsync();

                Assert.True(true);
                Assert.Equal(bookings.Count, await context.Booking.CountAsync());
                var repository = new BookingRepository(context);
                var booking = await repository.GetByIdAsync(expected.BookingId);
                Assert.IsType<Booking>(booking);
                Assert.Equal(expected, booking);
            }

        }

        [Fact]
        public async void UpdateAsync_UpdatesInContext()
        {
            var booking = BookingGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Booking.Add(booking);
                context.SaveChanges();
                var repository = new BookingRepository(context);
                var newBooking = await repository.GetByIdAsync(booking.BookingId);

                newBooking.IsBlock = true;

                await repository.UpdateAsync(newBooking);
                Assert.Equal(1, await context.Booking.CountAsync());
                Assert.Equal(newBooking, await context.Booking.SingleAsync());
            }


        }

        [Fact]
        public void GetBookingsInLocationAtDateTime()
        {
            var bookings = BookingGenerator.CreateList();
            bookings[1].BookingDateTime = bookings[2].BookingDateTime;
            bookings[1].Facility= bookings[2].Facility;
            bookings[1].FacilityId = bookings[2].FacilityId;

            var booking2Expected = bookings[2];
            List<Booking> expected = new List<Booking>();
            expected.Add(bookings[1]);
            expected.Add(bookings[2]);


            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Booking.AddRange(bookings);
                context.SaveChanges();
                var repository = new BookingRepository(context);
                var call = repository.GetBookingsInLocationAtDateTime(booking2Expected.BookingDateTime, booking2Expected.Facility.VenueId, booking2Expected.Facility.SportId).ToList();
                Assert.Equal(expected, call);
            }

        }

    }
}
