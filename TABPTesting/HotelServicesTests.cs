

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
    }
}
