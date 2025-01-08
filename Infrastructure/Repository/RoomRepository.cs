using Domain.Entities;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repository
{
    public class RoomRepository:Repository<Room>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(ApplicationDbContext context, ILogger<RoomRepository> logger)
            :base(context,logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task<PaginatedList<Room>> PaginateQueryAsync(IQueryable<Room> query, int pageNumber, int pageSize)
        {
            var totalItemCount = await query.CountAsync();
            var pageData = new PageData(totalItemCount, pageSize, pageNumber);

            var result = await query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PaginatedList<Room>(result, pageData);
        }

        public async Task<PaginatedList<Room>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Rooms.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                }
                return await PaginateQueryAsync(query, pageNumber, pageSize);
            }
            catch (Exception)
            {
                return new PaginatedList<Room>(new List<Room>(), new PageData(0, 0, 0));
            }
        }

        public async Task<PaginatedList<Room>> GetRoomsByHotelIdAsync(Guid hotelId, string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = (from roomType in _context.RoomTypes
                             join room in _context.Rooms on roomType.Id equals room.RoomTypeId
                             where roomType.HotelId == hotelId
                             select room);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                }

                return await PaginateQueryAsync(query, pageNumber, pageSize);
            }
            catch (Exception)
            {
                return new PaginatedList<Room>(new List<Room>(), new PageData(0, 0, 0));
            }
        }

        public async Task<bool> CheckRoomBelongsToHotelAsync(Guid hotelId, Guid roomId)
        {
            return await (from roomType in _context.RoomTypes
                          where roomType.HotelId.Equals(hotelId)
                          join room in _context.Rooms on
                          roomType.Id equals room.RoomTypeId
                          where room.Id.Equals(roomId)
                          select room)
                .AnyAsync();
        }

       

        

       
    }

}
