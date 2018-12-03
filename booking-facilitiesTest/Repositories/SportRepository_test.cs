using booking_facilities.Models;
using booking_facilitiesTest.TestUtils;
using booking_facilities.Repositories;
using Moq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

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

        }

    }
}
