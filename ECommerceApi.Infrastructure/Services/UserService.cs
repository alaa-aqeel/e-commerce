using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.DTOs.Profile;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IPasswordHasher hasher, IMapper mapper)
    {
        _context = context;
        _hasher = hasher;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<UserDto>>> GetAllUsersAsync(int page, int pageSize)
    {
        var query = _context.Users.AsNoTracking();
        var total = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return ServiceResult<PagedResult<UserDto>>.Ok(new PagedResult<UserDto>
        {
            Items = _mapper.Map<List<UserDto>>(items),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ServiceResult<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return ServiceResult<UserDto>.Fail("User not found.");
        return ServiceResult<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    public async Task<ServiceResult<UserDto>> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return ServiceResult<UserDto>.Fail("User not found.");
        return ServiceResult<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    public async Task<ServiceResult<UserDto>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return ServiceResult<UserDto>.Fail("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        await _context.SaveChangesAsync();
        return ServiceResult<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    public async Task<ServiceResult<bool>> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return ServiceResult<bool>.Fail("User not found.");
        if (!_hasher.Verify(request.CurrentPassword, user.PasswordHash))
            return ServiceResult<bool>.Fail("Current password is incorrect.");

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true, "Password changed successfully.");
    }

    public async Task<ServiceResult<bool>> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return ServiceResult<bool>.Fail("User not found.");
        user.IsDeleted = true;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
