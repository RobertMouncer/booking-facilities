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
    public class SportRepository_Test
    {
        private static readonly Random random = new Random();
        private readonly DbContextOptions<booking_facilitiesContext> contextOptions;

        public SportRepository_Test()
        {
            contextOptions = new DbContextOptionsBuilder<booking_facilitiesContext>()
                .UseInMemoryDatabase($"rand_db_name_{random.Next()}").Options;
        }

        [Fact]
        public async void AddAsync_AddAsyncToConext()
        {
            var sport = SportGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new SportRepository(context);
                await repository.AddAsync(sport);
                Assert.Equal(1, await context.Sport.CountAsync());
                Assert.Equal(sport, await context.Sport.SingleAsync());
            }
        }
        [Fact]

        public async void DoesSportExist_addSportReturnTrue()
        {
            var sport = SportGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new SportRepository(context);

                await repository.AddAsync(sport);
                Assert.True(repository.DoesSportExist(sport.SportName));
                
            }

        }

        [Fact]
        public async void DoesSportExist_addSportReturnFalse()
        {
            var sport = SportGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new SportRepository(context);
                await repository.AddAsync(sport);
                Assert.False(repository.DoesSportExist(sport.SportName + ' '));
            }

        }

        [Fact]
        public async void GetByIdAsync_ReturnsCorrectItems()
        {
            var list = SportGenerator.CreateList(5);
            var expected = list[2];

            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Sport.AddRange(list);
                
                await context.SaveChangesAsync();

                Assert.True(true);
                Assert.Equal(list.Count, await context.Sport.CountAsync());
                var repository = new SportRepository(context);
                var sport = await repository.GetByIdAsync(expected.SportId);
                Assert.IsType<Sport>(sport);
                Assert.Equal(expected, sport);
            }

        }

        [Fact]
        public async void UpdateAsync_UpdatesInContext()
        {
            var sport = SportGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Sport.Add(sport);
                context.SaveChanges();
                var repository = new SportRepository(context);
                var newSport = await repository.GetByIdAsync(sport.SportId);

                newSport.SportName = "Hockey";

                await repository.UpdateAsync(newSport);
                Assert.Equal(1, await context.Sport.CountAsync());
                Assert.Equal(newSport, await context.Sport.SingleAsync());
            }

        
        }

        [Fact]
        public async void DeleteAsync_DeleteFromContext()
        {
            var sport = SportGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                var repository = new SportRepository(context);
                context.Database.EnsureCreated();
                context.Sport.Add(sport);
                context.SaveChanges();
                Assert.Equal(1, await context.Sport.CountAsync());

                

                await repository.DeleteAsync(sport);
                Assert.Equal(0, await context.Sport.CountAsync());
            }
        }

        [Fact]
        public async void SportIdExists_ReturnTrue()
        {
            var sport = SportGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new SportRepository(context);
                await repository.AddAsync(sport);
                Assert.True(repository.SportIdExists(sport.SportId));
            }

        }

        [Fact]
        public async void SportIdExists_ReturnFalse()
        {
            var sport = SportGenerator.Create();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                var repository = new SportRepository(context);
                await repository.AddAsync(sport);
                Assert.False(repository.SportIdExists(sport.SportId+1));
            }

        }
        [Fact]
        public async void GetAllAsync_ReturnsAllFromContext()
        {
            var expectedSports = SportGenerator.CreateList();
            using (var context = new booking_facilitiesContext(contextOptions))
            {
                context.Database.EnsureCreated();
                context.Sport.AddRange(expectedSports);
                context.SaveChanges();

                Assert.Equal(expectedSports.Count, await context.Sport.CountAsync());

                var repository = new SportRepository(context);
                var resources = repository.GetAllAsync();

                Assert.IsAssignableFrom<IQueryable<Sport>>(resources);
                Assert.Equal(expectedSports, resources);
            }
        }
    }
}
