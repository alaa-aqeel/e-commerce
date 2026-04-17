using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Enums;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;
    private readonly IMapper _mapper;

    public AuthService(AppDbContext context, IPasswordHasher hasher, IJwtService jwt, IMapper mapper)
    {
        _context = context;
        _hasher = hasher;
        _jwt = jwt;
        _mapper = mapper;
    }

    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return ServiceResult<AuthResponse>.Fail("Email already registered.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower().Trim(),
            PasswordHash = _hasher.Hash(request.Password),
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Customer
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await BuildAuthResponseAsync(user);
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());
        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
            return ServiceResult<AuthResponse>.Fail("Invalid email or password.");

        return await BuildAuthResponseAsync(user);
    }

    public async Task<ServiceResult<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var userId = _jwt.GetUserIdFromExpiredToken(request.AccessToken);
        if (userId == null)
            return ServiceResult<AuthResponse>.Fail("Invalid access token.");

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            return ServiceResult<AuthResponse>.Fail("Invalid or expired refresh token.");

        return await BuildAuthResponseAsync(user);
    }

    public async Task<ServiceResult<bool>> LogoutAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return ServiceResult<bool>.Fail("User not found.");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private async Task<ServiceResult<AuthResponse>> BuildAuthResponseAsync(User user)
    {
        var userDto = _mapper.Map<UserDto>(user);
        var accessToken = _jwt.GenerateAccessToken(userDto);
        var refreshToken = _jwt.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return ServiceResult<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = userDto
        });
    }
}
