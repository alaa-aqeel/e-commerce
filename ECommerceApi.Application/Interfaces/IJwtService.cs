using ECommerceApi.Application.DTOs.Auth;

namespace ECommerceApi.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(UserDto user);
    string GenerateRefreshToken();
    int? GetUserIdFromExpiredToken(string token);
}
