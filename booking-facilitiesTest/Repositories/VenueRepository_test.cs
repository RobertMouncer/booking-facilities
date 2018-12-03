using booking_facilities.Models;
using booking_facilitiesTest.TestUtils;
using booking_facilities.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace booking_facilitiesTest.Repositories
{
    public class VenueRepository_Test
    {
        private static readonly Random random = new Random();
        private readonly DbContextOptions<booking_facilitiesContext> contextOptions;

        public VenueRepository_Test()
        {
            contextOptions = new DbContextOptionsBuilder<booking_facilitiesContext>()
                .UseInMemoryDatabase($"rand_db_name_{random.Next()}").Options;
        }
        [Fact]
        public async void AddAsync_AddsToConext()
        {
            var venue = VenueGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new VenueRepository(context);
                await repository.AddAsync(venue);
                Assert.Equal(1, await context.Venue.CountAsync());
                Assert.Equal(venue, await context.Venue.SingleAsync());
            }
        }
        [Fact]
        public async void DeleteAsync_RemovesFromContext()
        {
            var venue = VenueGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Venue.Add(venue);
                context.SaveChanges();
                Assert.Equal(1, await context.Venue.CountAsync());
                var repository = new VenueRepository(context);
                await repository.DeleteAsync(venue);
                Assert.Equal(0, await context.Venue.CountAsync());
            }
        }
        [Fact]
        public async void GetByIdAsync_ReturnsCorrectItems()
        {
            var list = VenueGenerator.CreateList(5);
            var expected = list[2];
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Venue.AddRange(list);
                context.SaveChanges();
                Assert.Equal(list.Count, await context.Venue.CountAsync());
                var repository = new VenueRepository(context);
                var venue = await repository.GetByIdAsync(expected.VenueId);
                Assert.IsType<Venue>(venue);
                Assert.Equal(expected, venue);
            }
        }
        [Fact]
        public async void UpdateAsync_UpdatesInContext()
        {
            var venue = VenueGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Venue.Add(venue);
                context.SaveChanges();
                var repository = new VenueRepository(context);
                var newVenue = await repository.GetByIdAsync(venue.VenueId);
                newVenue.VenueName = "Old Sports Hall";
                await repository.UpdateAsync(newVenue);
                Assert.Equal(1, await context.Venue.CountAsync());
                Assert.Equal(newVenue, await context.Venue.SingleAsync());
            }
        }
        [Fact]
        public void DoesVenueExist_ChecksByName()
        {
            var venue = VenueGenerator.Create();
            venue.VenueName = "King's College";
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Venue.Add(venue);
                context.SaveChanges();
                var repository = new VenueRepository(context);
                Assert.True(repository.DoesVenueExist("King's College"));
                Assert.False(repository.DoesVenueExist("Queens's College"));
            }
        }
        [Fact]
        public void VenueExists_ChecksById()
        {
            var venue = VenueGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Venue.Add(venue);
                context.SaveChanges();
                var repository = new VenueRepository(context);
                Assert.True(repository.VenueExists(venue.VenueId));
                Assert.False(repository.VenueExists(-1));
                Console.WriteLine(venue.VenueId);
            }
        }
        [Fact]
        public async void GetAllAsync_ReturnsAllFromContext()
        {
            var expectedVenues = VenueGenerator.CreateList();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Venue.AddRange(expectedVenues);
                context.SaveChanges();

                Assert.Equal(expectedVenues.Count, await context.Venue.CountAsync());

                var repository = new VenueRepository(context);
                var resources = repository.GetAllAsync();

                Assert.IsAssignableFrom<IQueryable<Venue>>(resources);
                Assert.Equal(expectedVenues, resources);
            }
        }
    }
}
