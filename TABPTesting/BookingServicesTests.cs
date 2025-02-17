using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.BookingDTOs;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

public class BookingServicesTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly Mock<IRoomRepository> _mockRoomRepo;
    private readonly Mock<IPaymentRepository> _mockPaymentRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ApplicationDbContext _context; // Use real DbContext with in-memory database
    private readonly BookingServices _bookingServices;

    public BookingServicesTests()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _context = new ApplicationDbContext(options);

        // Initialize repositories and services
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockRoomRepo = new Mock<IRoomRepository>();
        _mockPaymentRepo = new Mock<IPaymentRepository>();
        _mockMapper = new Mock<IMapper>();

        _bookingServices = new BookingServices(
            _mockBookingRepo.Object,
            _mockRoomRepo.Object,
            _mockPaymentRepo.Object,
            _mockMapper.Object,
            _context // Use real context with in-memory database
        );
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsBookings_WhenBookingsExist()
    {
        // Arrange
        var bookings = new List<Booking> { new Booking(), new Booking() };
        _mockBookingRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(bookings);

        // Act
        var result = await _bookingServices.GetAllBookingsAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllBookingsAsync_ThrowsNotFoundException_WhenNoBookings()
    {
        // Arrange
        _mockBookingRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Booking>());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _bookingServices.GetAllBookingsAsync());
    }
    [Fact]
    public async Task GetBookingByIdAsync_ReturnsBookingDto_WhenBookingExists()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking { Id = bookingId };
        var bookingDto = new BookingByIdDTO { Id = bookingId };

        _mockBookingRepo.Setup(repo => repo.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _mockMapper.Setup(m => m.Map<BookingByIdDTO>(booking)).Returns(bookingDto);

        // Act
        var result = await _bookingServices.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.Equal(bookingId, result.Id);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _mockBookingRepo.Setup(repo => repo.GetByIdAsync(bookingId)).ReturnsAsync((Booking)null);

        // Act
        var result = await _bookingServices.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.Null(result);
    }


}