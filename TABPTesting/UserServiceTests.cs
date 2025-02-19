

using Application.DTOs.UserDTOs;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System.Data.SqlTypes;

namespace TABPTesting
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockMapper = new Mock<IMapper>();
            _userService = new UserService(
                                _mockUserRepo.Object, 
                                _mockPasswordHasher.Object, 
                                _mockMapper.Object);
        }
        [Fact]
        public async Task RegisterUserAsync_ShouldCreateUserWithHashedPassword_WhenValidDto()
        {
            // Arrange
            var userDto = new UserRegisterDTO { Password = "securePassword123!" };
            var userEntity = new User();
            var salt = new byte[] { 1, 2, 3 };
            var expectedSalt= Convert.ToBase64String(salt);
            var expectedHash = "hashed_password";

            _mockMapper.Setup(m => m.Map<User>(userDto))
                .Returns(userEntity);

            _mockPasswordHasher.Setup(p => p.GenerateSalt())
                .Returns(expectedSalt);

            _mockPasswordHasher.Setup(p => p.HashPassword(userDto.Password, expectedSalt))
                .Returns(expectedHash);

            // Act
            await _userService.RegisterUserAsync(userDto);

            // Assert
            _mockPasswordHasher.Verify(p => p.GenerateSalt(), Times.Once);
            _mockPasswordHasher.Verify(p => p.HashPassword(userDto.Password, expectedSalt), Times.Once);

            _mockUserRepo.Verify(r => r.InsertAsync(It.Is<User>(u =>
                u.Salt == expectedSalt &&
                u.PasswordHash == expectedHash
            )), Times.Once);
        }


    }
}
