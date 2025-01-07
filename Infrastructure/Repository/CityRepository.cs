

using Domain.Entities;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    public class CityRepository:Repository<City> 
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CityRepository> _logger;

        public CityRepository(ApplicationDbContext context, ILogger<CityRepository> logger)
            :base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PaginatedList<City>> GetAllAsync(bool includeHotels, string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Cities.AsQueryable();
                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    query = query.Where
                            (city => city.Name.Contains(searchQuery) ||
                            city.Country.Contains(searchQuery));
                }

                if (includeHotels)
                {
                    query = query.Include(city => city.Hotels);
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<City>(result, pageData);
            }
            catch (Exception)
            {
                return new PaginatedList<City>(
                    new List<City>(),
                    new PageData(0, 0, 0));
            }
        }

    }
}
