
using Application.DTOs.CityDTOs;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TABPTesting
{
    public class CityServicesTest
    {
        private readonly Mock<ICityRepository> _mockCityRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ApplicationDbContext _context;
        private readonly CityServices _cityServices;
        public CityServicesTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _mockCityRepo = new Mock<ICityRepository>();
            _mockMapper = new Mock<IMapper>();

            _context = new ApplicationDbContext(options);
            _cityServices = new CityServices(
                               _mockCityRepo.Object,
                                              _mockMapper.Object
                                                         );
        }

        [Fact]
        public async Task GetCitiesWithHotelsAsync_ReturnsCities_WhenCitiesExist()
        {
            // Arrange
            const string searchQuery = "s";
            const int pageNumber = 1;
            const int pageSize = 5;

            var pageData = new PageData(
                totalItemCount: 2,
                pageSize: pageSize, 
                currentPage: pageNumber
            );

            var cities = new List<City> { new City(), new City() };
            var paginatedCities = new PaginatedList<City>(
                items: cities,
                pageData: pageData
            );

            // Mock repository response
            _mockCityRepo.Setup(repo =>
                repo.GetAllAsync(true, searchQuery, pageNumber, pageSize)
            ).ReturnsAsync(paginatedCities);

            var cityDtos = new List<CityDTO> { new CityDTO(), new CityDTO() };
            _mockMapper.Setup(m => m.Map<List<CityDTO>>(It.IsAny<List<City>>()))
                .Returns(cityDtos);

            // Act
            var result = await _cityServices.GetCitiesWithHotelsAsync(
                searchQuery,
                pageNumber,
                pageSize
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(pageNumber, result.PageData.CurrentPage);
        }
        [Fact]
        public async Task GetCitiesWithHotelsAsync_ThrowsNotFoundException_WhenNoCities()
        {
            // Arrange
            const string searchQuery = "s";
            const int pageNumber = 1;
            const int pageSize = 5;

            var pageData = new PageData(
                totalItemCount: 0,
                pageSize: pageSize,
                currentPage: pageNumber
            );

            var cities = new List<City>(); // Empty list
            var paginatedCities = new PaginatedList<City>(cities, pageData);

            _mockCityRepo.Setup(repo =>
                repo.GetAllAsync(true, searchQuery, pageNumber, pageSize)
            ).ReturnsAsync(paginatedCities);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _cityServices.GetCitiesWithHotelsAsync(searchQuery, pageNumber, pageSize)
            );
        }

        [Fact]
        public async Task GetCitiesWithoutHotelsAsync_ReturnsCities_WhenCitiesExist()
        {
            // Arrange
            var cities = new List<City> { new City(), new City() };
            var paginatedCities = new PaginatedList<City>(
                cities,
                new PageData(2, 5, 1)
            );

            _mockCityRepo.Setup(r => r.GetAllAsync(false, It.IsAny<string>(), 1, 5))
                .ReturnsAsync(paginatedCities);
            _mockMapper.Setup(m => m.Map<List<CityDTOWithoutHotels>>(It.IsAny<List<City>>()))
                .Returns(new List<CityDTOWithoutHotels> { new(), new() });

            // Act
            var result = await _cityServices.GetCitiesWithOutHotelsAsync(null, 1, 5);

            // Assert
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task GetCityByIdAsync_ReturnsCity_WhenExists()
        {
            // Arrange
            var cityId = Guid.NewGuid();
            var city = new City { Id = cityId };
            _mockCityRepo.Setup(r => r.GetByIdAsync(cityId, true))
                .ReturnsAsync(city);

            // Act
            var result = await _cityServices.GetCityByIdAsync(cityId, true);

            // Assert
            Assert.Equal(cityId, result.Id);
        }

        [Fact]
        public async Task GetCityByIdAsync_ThrowsKeyNotFoundException_WhenNotExists()
        {
            // Arrange
            var cityId = Guid.NewGuid();
            _mockCityRepo.Setup(r => r.GetByIdAsync(cityId, It.IsAny<bool>()))
                .ReturnsAsync((City)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _cityServices.GetCityByIdAsync(cityId));
        }
        [Fact]
        public async Task AddCityAsync_ThrowsArgumentNull_WhenDtoIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _cityServices.AddCityAsync(null));
        }
    }
}
