

using Domain.Entities;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository
{
    internal class RoomTypeRepository:Repository<RoomType>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomTypeRepository> _logger;

        public RoomTypeRepository(ApplicationDbContext context, ILogger<RoomTypeRepository> logger)
            :base(context, logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<RoomType>> GetAllAsync(Guid hotelId, bool includeAmenities, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context
                    .RoomTypes
                    .Where(roomType => roomType
                        .HotelId
                        .Equals(hotelId))
                    .AsQueryable();

                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (includeAmenities)
                {
                    query = query.
                        Include(roomCategory =>
                        roomCategory.Amenities);
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<RoomType>(result, pageData);
            }
            catch (Exception)
            {
                return new PaginatedList<RoomType>(
                    new List<RoomType>(),
                    new PageData(0, 0, 0));
            }
        }

       

        public async Task<bool> CheckRoomTypeExistenceForHotel(Guid hotelId, Guid roomTypeId)
        {
            return (await GetByIdAsync(roomTypeId))
                   .HotelId.Equals(hotelId);
        }

       

       
    }

}
