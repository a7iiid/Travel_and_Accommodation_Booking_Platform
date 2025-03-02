using Application.DTOs;
using Application.DTOs.UserDTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper,ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        }

        public async Task RegisterUserAsync(UserRegisterDTO userDto)
        {
            var userEntity = _mapper.Map<User>(userDto);
            userEntity.Salt = _passwordHasher.GenerateSalt();
            userEntity.PasswordHash = _passwordHasher.HashPassword(userDto.Password, userEntity.Salt);

            await _userRepository.InsertAsync(userEntity);
        }
        
        public async Task<string> AuthenticateUserAsync(AuthenticationRequestDTO authenticationRequestBody)
        {
            var user = await _tokenGenerator.ValidateUserCredentials(
                authenticationRequestBody.Email,
                authenticationRequestBody.Password);

            if (user is null)
            {
                   throw new NotFoundException("Invalid email or password.");

            }

          
            // Generate the JWT token
            var token = await _tokenGenerator.GenerateToken(
                user);
            return token;

        }

    }

}
