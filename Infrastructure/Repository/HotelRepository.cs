﻿using Domain.Entities;
using Domain.Exceptions;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Infrastructure.Repository
{
    public class HotelRepository : Repository<Hotel>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HotelRepository> _logger;

        public HotelRepository(ApplicationDbContext context, ILogger<HotelRepository> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<PaginatedList<Hotel>> GetAllAsync(string? searchQuery, int pageNumber, int pageSize)
        {
            try
            {
                
                var query = _context.Hotels.AsQueryable();
                var totalItemCount = await query.CountAsync();
                var pageData = new PageData(totalItemCount, pageSize, pageNumber);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    searchQuery = searchQuery.Trim();
                    query = query.Where
                    (city => city.Name.Contains(searchQuery) ||
                             city.Description.Contains(searchQuery) ||
                             city.StreetAddress.Contains(searchQuery)
                    );
                }

                var result = query
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToList();

                return new PaginatedList<Hotel>(result, pageData);
            }
            catch (Exception)
            {
                return new PaginatedList<Hotel>(new List<Hotel>(), new PageData(0, 0, 0));
            }
        }
        private bool IsRoomAvailable(Guid roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var roomBookings = _context
                .Bookings
                .Where(b => b.RoomId.Equals(roomId))
                .ToList();

            return roomBookings.All(booking =>
                checkInDate.Date > booking.CheckOutDate.Date ||
                checkOutDate.Date < booking.CheckInDate.Date);
        }
        public async Task<List<Room>> GetHotelAvailableRoomsAsync(
            Guid hotelId,
            DateTime checkInDate,
            DateTime checkOutDate)
        {
            var rooms = await (from hotel in _context.Hotels
                               join roomType in _context.RoomTypes on hotel.Id equals roomType.HotelId
                               join room in _context.Rooms on roomType.Id equals room.RoomTypeId
                               where roomType.HotelId.Equals(hotelId)
                               select new Room
                               {
                                   Id = room.Id,
                                   RoomTypeId = room.RoomTypeId,
                                   AdultsCapacity = room.AdultsCapacity,
                                   ChildrenCapacity = room.ChildrenCapacity,
                                   Rating = room.Rating,
                                   RoomType = roomType 
                               }).ToListAsync();
           





            return rooms.Where(room =>
                IsRoomAvailable(
                room.Id,
                checkInDate,
                checkOutDate)).ToList();
        }

        public async Task<PaginatedList<HotelSearchResult>> HotelSearchAsync(HotelSearchParameters searchParams)
        {

            var cityFilterQuery = _context.Cities.AsQueryable();

            if (searchParams.CityName is not null)
            {
                cityFilterQuery = cityFilterQuery
                    .Where(city => city
                        .Name.ToLower()
                        .Contains(searchParams.CityName.Trim().ToLower()));
            }

            var roomFilterQuery = FindAvailableRoomsWithCapacity(
                searchParams.Adults,
                searchParams.Children,
                searchParams.CheckInDate,
                searchParams.CheckOutDate);

            var hotelFilterQuery = from hotel in _context.Hotels
                                   where hotel.Rating >= searchParams.StarRate
                                   select hotel;

            var query = from city in cityFilterQuery
                        join hotel in hotelFilterQuery on city.Id equals hotel.CityId
                        join roomType in _context.RoomTypes on hotel.Id equals roomType.HotelId
                        join room in roomFilterQuery on roomType.Id equals room.RoomTypeId
                        select new HotelSearchResult
                        {
                            CityId = city.Id,
                            CityName = city.Name,
                            HotelId = hotel.Id,
                            HotelName = hotel.Name,
                            Rating = hotel.Rating,
                            RoomId = room.Id,
                            RoomPricePerNight = roomType.PricePerNight,
                            RoomType = roomType.Category.ToString()
                        };

            var totalItemCount = await query.CountAsync();
            var pageData = new PageData(totalItemCount, searchParams.PageSize, searchParams.PageNumber);

            var result = await query
                .Skip(searchParams.PageSize * (searchParams.PageNumber - 1))
                .Take(searchParams.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PaginatedList<HotelSearchResult>(result, pageData);
        }

      
        private IQueryable<Room> FindAvailableRoomsWithCapacity(int adults,int children, DateTime checkInDate, DateTime checkOutDate)
        {
            return from room in _context.Rooms
                   where room.AdultsCapacity == adults &&
                          room.ChildrenCapacity==children &&
                         _context.Bookings.Where(booking => booking.RoomId == room.Id).All
                         (booking => checkInDate.Date > booking.CheckOutDate.Date ||
                         checkOutDate.Date < booking.CheckInDate.Date)
                   select room;
        }
        public async Task<PaginatedList<Hotel>> GetHotelsByOwnerIdAsync(
     Guid ownerId,
     int PageSize = 5,
     int PageNumber = 1)
        {
            try
            {
                // Create base query
                var baseQuery = _context.Hotels
                    .Where(h => h.OwnerId == ownerId)
                    .Include(h => h.City)
                    .AsNoTracking();

                // Get total count from database
                var totalItemCount = await baseQuery.CountAsync();

                // Apply pagination and execute query
                var items = await baseQuery
                    .Skip(PageSize * (PageNumber - 1))
                    .Take(PageSize)
                    .ToListAsync();

                // Create page data
                var pageData = new PageData(totalItemCount, PageSize, PageNumber);

                return new PaginatedList<Hotel>(items, pageData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotels for owner {OwnerId}", ownerId);
                throw new DataAccessException("Failed to retrieve hotels", ex);
            }
        }
    }
}
