using Domain.Entities;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICityRepository
    {
        public Task<PaginatedList<City>>GetAllAsync(bool includeHotels,
                                string? searchQuery,
                                int pageNumber,
                                int pageSize);
        public Task<City?> GetByIdAsync(Guid cityId, bool includeHotels);
        public Task<City?> InsertAsync(City city);
        public Task UpdateAsync(City city);
        public Task<bool> DeleteAsync(Guid cityId);
        public Task SaveChangesAsync();
        public Task<bool> IsExistsAsync(Guid cityId);
    }
}
