using booking_facilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Repositories
{
    public interface ISportRepository
    {
        IQueryable<Sport> GetAllAsyncOrderByDate();
        Task<Sport> GetByIdAsync(int id);
        bool DoesSportExist(string sportname);
        Task<Sport> AddAsync(Sport sport);
        Task<Sport> UpdateAsync(Sport sport);
        Task<Sport> DeleteAsync(Sport sport);
        bool sportIdExist(int id);
    }
}
