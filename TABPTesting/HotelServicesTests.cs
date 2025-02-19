

using Application.DTOs.HotelDTOs;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Model;
using Moq;

namespace TABPTesting
{
    public class HotelServicesTests
    {
        private readonly Mock<IHotelRepository> _mockHotelRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly HotelServices _hotelServices;
        public HotelServicesTests()
        {
            _mockHotelRepo = new Mock<IHotelRepository>();
            _mockMapper = new Mock<IMapper>();
            _hotelServices = new HotelServices(_mockHotelRepo.Object, _mockMapper.Object);
        }
        [Fact]
        public async Task GetHotelsAsync_ReturnsPaginatedList_WhenHotelsExist()
        {
            // Arrange
            var hotels = new List<Hotel> { new Hotel(), new Hotel() };
            var paginatedHotels = new PaginatedList<Hotel>(hotels, new PageData(2, 10, 1));

            _mockHotelRepo.Setup(r => r.GetAllAsync(It.IsAny<string>(), 1, 10))
                .ReturnsAsync(paginatedHotels);

            // Act
            var result = await _hotelServices.GetHotelsAsync(null, 1, 10);

            // Assert
            Assert.Equal(2, result.Items.Count);
        }
        [Fact]
        public async Task GetHotelByIdAsync_ReturnsHotelDto_WhenHotelExists()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var hotel = new Hotel { Id = hotelId };
            var hotelDto = new HotelDTO();

            _mockHotelRepo.Setup(r => r.GetByIdAsync(hotelId))
                .ReturnsAsync(hotel);
            _mockMapper.Setup(m => m.Map<HotelDTO>(hotel))
                .Returns(hotelDto);

            // Act
            var result = await _hotelServices.GetHotelByIdAsync(hotelId);

            // Assert
            Assert.Equal(hotelDto, result);
        }

        [Fact]
        public async Task GetHotelByIdAsync_ThrowsKeyNotFoundException_WhenHotelMissing()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            _mockHotelRepo.Setup(r => r.GetByIdAsync(hotelId))
                .ReturnsAsync((Hotel)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _hotelServices.GetHotelByIdAsync(hotelId));
        }

        [Fact]
        public async Task GetAvailableRoomsAsync_ReturnsMappedRooms()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var rooms = new List<Room> { new Room(), new Room() };
            var checkIn = DateTime.Now.AddDays(1);
            var checkOut = DateTime.Now.AddDays(3);

            _mockHotelRepo.Setup(r => r.GetHotelAvailableRoomsAsync(hotelId, checkIn, checkOut))
                .ReturnsAsync(rooms);
            _mockMapper.Setup(m => m.Map<List<Room>>(rooms))
                .Returns(rooms);

            // Act
            var result = await _hotelServices.GetAvailableRoomsAsync(hotelId, checkIn, checkOut);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchHotelsAsync_ReturnsHotelSearchResults()
        {
            // Arrange
            var searchParams = new HotelSearchParameters();
            

            var expectedResults = new List<HotelSearchResult> { new HotelSearchResult(), new HotelSearchResult() };
            var expectedPaginatedResults = new PaginatedList<HotelSearchResult>(expectedResults, new PageData(2, 10, 1));

            _mockHotelRepo.Setup(r => r.HotelSearchAsync(searchParams))
                .ReturnsAsync(expectedPaginatedResults);

            _mockMapper.Setup(_mockMapper =>
            _mockMapper.Map<List<HotelSearchResult>>(expectedResults))
                .Returns(expectedResults);


            

            // Act
            var result = await _hotelServices.SearchHotelsAsync(searchParams);

            // Assert
            Assert.Equal(2, result.Items.Count);
            Assert.IsType<PaginatedList<HotelSearchResult>>(result);
        }

        [Fact]
        public async Task AddHotelAsync_CreatesNewHotel()
        {
            // Arrange
            var hotelDto = new HotelDTO();
            var hotelEntity = new Hotel();

            _mockMapper.Setup(m => m.Map<Hotel>(hotelDto))
                .Returns(hotelEntity);

            // Act
            await _hotelServices.AddHotelAsync(hotelDto);

            // Assert
            _mockHotelRepo.Verify(r => r.InsertAsync(hotelEntity), Times.Once);
        }

        [Fact]
        public async Task UpdateHotelAsync_UpdatesExistingHotel()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var existingHotel = new Hotel { Id = hotelId };
            var hotelDto = new HotelDTO();

            _mockHotelRepo.Setup(r => r.GetByIdAsync(hotelId))
                .ReturnsAsync(existingHotel);

            // Act
            await _hotelServices.UpdateHotelAsync(hotelId, hotelDto);

            // Assert
            _mockHotelRepo.Verify(r => r.UpdateAsync(existingHotel, hotelId), Times.Once);
        }

        [Fact]
        public async Task DeleteHotelAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            _mockHotelRepo.Setup(r => r.DeleteAsync(hotelId))
                .ReturnsAsync(true);

            // Act
            var result = await _hotelServices.DeleteHotelAsync(hotelId);

            // Assert
            Assert.True(result);
        }

    }
}
