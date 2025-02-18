using Application.DTOs.UserDTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repository;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task RegisterUserAsync(UserRegisterDTO userDto)
        {
            var userEntity = _mapper.Map<User>(userDto);
            userEntity.Salt = _passwordHasher.GenerateSalt();
            userEntity.PasswordHash = _passwordHasher.HashPassword(userDto.Password, userEntity.Salt);

            await _userRepository.InsertAsync(userEntity);
        }
    }
}
