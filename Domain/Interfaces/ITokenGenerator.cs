using Infrastructure.Auth.model;

namespace Domain.Interfaces
{
    public interface ITokenGenerator
    {
        Task<string> GenerateToken(string email, string password);
        Task<UserResultModel?> ValidateUserCredentials(string email, string password);
    }
}