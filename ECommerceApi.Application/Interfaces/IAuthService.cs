using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Auth;

namespace ECommerceApi.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<ServiceResult<bool>> LogoutAsync(int userId);
}
