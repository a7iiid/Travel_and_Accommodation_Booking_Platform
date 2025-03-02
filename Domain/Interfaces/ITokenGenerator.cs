using Infrastructure.Auth.model;

namespace Domain.Interfaces
{
    public interface ITokenGenerator
    {
        Task<string> GenerateToken(UserResultModel user);
        Task<UserResultModel?> ValidateUserCredentials(string email, string password);
    }
}