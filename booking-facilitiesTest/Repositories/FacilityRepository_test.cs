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
    public class FacilityRepository_Test
    {
        private static readonly Random random = new Random();
        private readonly DbContextOptions<booking_facilitiesContext> contextOptions;

        public FacilityRepository_Test()
        {
            contextOptions = new DbContextOptionsBuilder<booking_facilitiesContext>()
                .UseInMemoryDatabase($"rand_db_name_{random.Next()}").Options;
        }
        [Fact]
        public async void AddAsync_AddsToConext()
        {
            var facility = FacilityGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new FacilityRepository(context);
                await repository.AddAsync(facility);
                Assert.Equal(1, await context.Facility.CountAsync());
                Assert.Equal(facility, await context.Facility.SingleAsync());
            }
        }
        [Fact]
        public async void DeleteAsync_RemovesFromContext()
        {
            var facility = FacilityGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Facility.Add(facility);
                context.SaveChanges();
                Assert.Equal(1, await context.Facility.CountAsync());
                var repository = new FacilityRepository(context);
                await repository.DeleteAsync(facility);
                Assert.Equal(0, await context.Facility.CountAsync());
            }
        }
        [Fact]
        public async void GetByIdAsync_ReturnsCorrectItems()
        {
            var list = FacilityGenerator.CreateList(5);
            var expected = list[2];
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Facility.AddRange(list);
                context.SaveChanges();
                Assert.Equal(list.Count, await context.Facility.CountAsync());
                var repository = new FacilityRepository(context);
                var facility = await repository.GetByIdAsync(expected.FacilityId);
                Assert.IsType<Facility>(facility);
                Assert.Equal(expected, facility);
            }
        }
        [Fact]
        public async void UpdateAsync_UpdatesInContext()
        {
            var facility = FacilityGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Facility.Add(facility);
                context.SaveChanges();
                var repository = new FacilityRepository(context);
                var newFacility = await repository.GetByIdAsync(facility.FacilityId);
                newFacility.FacilityName = "Court 1";
                newFacility.SportId = 2;
                Assert.Equal(1, await context.Facility.CountAsync());
                Assert.Equal(newFacility, await context.Facility.SingleAsync());
            }
        }
        [Fact]
        public void FacilityExists_ChecksById()
        {
            var facility = FacilityGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Facility.Add(facility);
                context.SaveChanges();
                var repository = new FacilityRepository(context);
                Assert.True(repository.FacilityExists(facility.FacilityId));
                Assert.False(repository.FacilityExists(-1));
                Console.WriteLine(facility.FacilityId);
            }
        }
        [Fact]
        public async void GetAllAsync_ReturnsAllFromContext()
        {
            var expectedFacilitys = FacilityGenerator.CreateList();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Facility.AddRange(expectedFacilitys);
                context.SaveChanges();

                Assert.Equal(expectedFacilitys.Count, await context.Facility.CountAsync());

                var repository = new FacilityRepository(context);
                var resources = repository.GetAllAsync();

                Assert.IsAssignableFrom<IQueryable<Facility>>(resources);
                Assert.Equal(expectedFacilitys, resources);
            }
        }
    }
}
