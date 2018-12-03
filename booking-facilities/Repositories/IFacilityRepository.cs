using booking_facilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace booking_facilities.Repositories
{
    public interface IFacilityRepository
    {
        Task<Facility> GetByIdAsync(int id);
        Task<Facility> AddAsync(Facility facility);
        Task<Facility> UpdateAsync(Facility facility);
        Task<Facility> DeleteAsync(Facility facility);
        bool FacilityExists(int id);
        IQueryable<Facility> GetAllAsync();
    }
}
