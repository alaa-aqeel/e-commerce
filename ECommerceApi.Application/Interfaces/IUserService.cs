using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.DTOs.Profile;

namespace ECommerceApi.Application.Interfaces;

public interface IUserService
{
    Task<ServiceResult<PagedResult<UserDto>>> GetAllUsersAsync(int page, int pageSize);
    Task<ServiceResult<UserDto>> GetUserByIdAsync(int id);
    Task<ServiceResult<UserDto>> GetProfileAsync(int userId);
    Task<ServiceResult<UserDto>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<ServiceResult<bool>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<ServiceResult<bool>> DeleteUserAsync(int id);
}
