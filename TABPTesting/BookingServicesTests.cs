
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using FluentAssertions;
using Infrastructure.DB;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TABPTesting
{
    public class BookingServicesTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IRoomRepository> _mockRoomRepository;
        private readonly Mock<IPaymentRepository> _mockPaymentServices;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly BookingServices _bookingServices;

        public BookingServicesTests()
        {
            // Mock dependencies
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockPaymentServices = new Mock<IPaymentRepository>();
            _mockMapper = new Mock<IMapper>();

            // Configure in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize the service with mocked dependencies
            _bookingServices = new BookingServices(
                _mockBookingRepository.Object,
                _mockRoomRepository.Object,
                _mockPaymentServices.Object,
                _mockMapper.Object,
                new ApplicationDbContext(_dbContextOptions)
            );
        }

       
        [Fact]
        public async Task GetAllBookingsAsync_ReturnsBookings_WhenBookingsExist()
        {
            // Arrange
            var bookings = new List<Booking> { new Booking(), new Booking() };
            _mockBookingRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(bookings);

            // Act
            var result = await _bookingServices.GetAllBookingsAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllBookingsAsync_ThrowsNotFoundException_WhenNoBookings()
        {
            // Arrange
            _mockBookingRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Booking>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _bookingServices.GetAllBookingsAsync());
        }
    }
}
