﻿using booking_facilities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Repositories
{
    public class SportRepository : ISportRepository
    {
        private readonly booking_facilitiesContext context;

        public SportRepository(booking_facilitiesContext context)
        {
            this.context = context;
        }

        public async Task<Sport> GetByIdAsync(int id)
        {
            return await context.Sport.FirstOrDefaultAsync(s => s.SportId == id);
        }

        public bool DoesSportExist(string sportname)
        {
            if (context.Sport.Any(s => s.SportName == sportname))
            {
                return true;
            }
            return false;
        }

        public async Task<Sport> AddAsync(Sport sport)
        {
            context.Add(sport);
            await context.SaveChangesAsync();
            return sport;
        }
        public async Task<Sport> UpdateAsync(Sport sport)
        {
            context.Update(sport);
            await context.SaveChangesAsync();
            return sport;
        }
        public async Task<Sport> DeleteAsync(Sport sport)
        {
            context.Sport.Remove(sport);
            await context.SaveChangesAsync();
            return sport;
        }

        public bool SportIdExists(int id)
        {
            return context.Sport.Any(e => e.SportId == id);
        }

        public IQueryable<Sport> GetAllAsync()
        {
            return  context.Sport;
        }

    }
}