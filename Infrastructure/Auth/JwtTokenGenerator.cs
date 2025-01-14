using Domain.Model;
using Infrastructure.Auth.model;
using Infrastructure.Auth.password;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IAuthUser _authUser;
        private readonly IPasswordHasher _passwordGenerator;
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IAuthUser authUser, IPasswordHasher passwordGenerator, IConfiguration configuration)
        {
            _authUser = authUser;
            _passwordGenerator = passwordGenerator;
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(string email, string password,JWTConfig jWTConfig)
        {
            var user = await ValidateUserCredentials(email, password);
            var key = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(jWTConfig.SecretKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
        {
            new("Email", user.Email),
            new("isAdmin", user.IsAdmin.ToString()),
            new("Name", user.FirstName + " " + user.LastName)
        };

            var jwtSecurityToken = new JwtSecurityToken(
                jWTConfig.Issuer,
                 jWTConfig.Audience,
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow
                .AddMinutes(30),
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public async Task<User?> ValidateUserCredentials(string email, string password)
        {
            var user = await _authUser.GetUserAsync(email);
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
