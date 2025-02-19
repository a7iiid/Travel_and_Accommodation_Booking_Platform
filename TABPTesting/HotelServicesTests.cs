

using Application.Services;
using AutoMapper;
using Domain.Interfaces;
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
    }
}
