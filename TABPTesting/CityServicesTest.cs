
using Application.DTOs.CityDTOs;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TABPTesting
{
    public class CityServicesTest
    {
        private readonly Mock<ICityRepository> _cityRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ApplicationDbContext _context;
        private readonly CityServices _cityServices;
        public CityServicesTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _cityRepository = new Mock<ICityRepository>();
            _mockMapper = new Mock<IMapper>();

            _context = new ApplicationDbContext(options);
            _cityServices = new CityServices(
                               _cityRepository.Object,
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
            _cityRepository.Setup(repo =>
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
    }
}
