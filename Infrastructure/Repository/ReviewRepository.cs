using Domain.Entities;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repository
{
    public class ReviewRepository:Repository<Review>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReviewRepository> _logger;

        public ReviewRepository(ApplicationDbContext context, ILogger<ReviewRepository> logger)
            :base(context, logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedList<Review>> GetAllByHotelIdAsync(Guid hotelId, string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                var query = (from booking in _context.Bookings
                             join room in _context.Rooms on booking.RoomId equals room.Id
                             join roomType in _context.RoomTypes on room.RoomTypeId equals roomType.Id
                             join hotel in _context.Hotels on roomType.HotelId equals hotel.Id
                             join review in _context.Reviews on booking.Id equals review.BookingId
                             where roomType.HotelId == hotelId
                             select review)
                        .AsQueryable()
                        .AsNoTracking();

                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    query = query.Where
                        (review => review.Comment.Contains(searchQuery));
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<Review>(result, pageData);
            }
            catch (Exception)
            {
                _logger.LogWarning("Data Access Exception ");
                return new PaginatedList<Review>(new List<Review>(),
                new PageData(0, 0, 0));
            }
        }

        

      

        

       

        

      
    }
}
