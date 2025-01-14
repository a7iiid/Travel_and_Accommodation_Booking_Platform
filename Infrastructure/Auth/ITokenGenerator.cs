using Domain.Model;
using Infrastructure.Auth.model;

namespace Infrastructure.Auth
{
    public interface ITokenGenerator
    {
        Task<string> GenerateToken(string email, string password,JWTConfig jWTConfig);
        Task<User?> ValidateUserCredentials(string email, string password);
    }
}