

using Domain.Entities;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    public class RoomAmenityRepository:Repository<RoomAmenity>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomAmenityRepository> _logger;

        public RoomAmenityRepository(ApplicationDbContext context, ILogger<RoomAmenityRepository> logger)
            :base(context,logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<RoomAmenity>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.RoomAmenities.AsQueryable();
                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    query = query.Where
                    (city => city.Name.Contains(searchQuery) ||
                    city.Description.Contains(searchQuery));
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<RoomAmenity>(result, pageData);
            }
            catch (Exception)
            {
                _logger.LogWarning("Data Access Exception");
                return new PaginatedList<RoomAmenity>(new List<RoomAmenity>(), new PageData(0, 0, 0));
            }
        }

        
    }

}
