using Domain.Interfaces;
using Domain.Model;
using Infrastructure.Auth.model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Interfaces;

namespace Infrastructure.Auth
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordGenerator;
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IUserRepository authUser, IPasswordHasher passwordGenerator, IConfiguration configuration)
        {
            _userRepository = authUser;
            _passwordGenerator = passwordGenerator;
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(string email, string password)
        {
            var user = await ValidateUserCredentials(email, password);
               

            var key = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretKey")));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
        {
            new ("Id", user.Id.ToString()), // UserId

            new("Email", user.Email),
            new("isAdmin", user.IsAdmin.ToString()),
            new("Name", user.FirstName + " " + user.LastName)
        };

            var jwtSecurityToken = new JwtSecurityToken(
                 Environment.GetEnvironmentVariable("Issuer"),
                 Environment.GetEnvironmentVariable("Audience"),
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow
                .AddMinutes(30),
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public async Task<UserResultModel?> ValidateUserCredentials(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user is null)
            {
                return null;
            }
            var isPasswordMatch = _passwordGenerator
                                  .VerifyPassword(
                                  password,
                                  user.Salt,
                                  user.Password);
            return isPasswordMatch ? user : null;
        }
    }
}
