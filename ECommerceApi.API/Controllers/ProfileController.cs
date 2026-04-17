using ECommerceApi.Application.DTOs.Profile;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/profile")]
[Authorize]
public class ProfileController : BaseController
{
    private readonly IUserService _users;

    public ProfileController(IUserService users) => _users = users;

    /// <summary>Get the current user's profile</summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile() =>
        HandleResult(await _users.GetProfileAsync(CurrentUserId));

    /// <summary>Update the current user's profile</summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request) =>
        HandleResult(await _users.UpdateProfileAsync(CurrentUserId, request));

    /// <summary>Change the current user's password</summary>
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request) =>
        HandleResult(await _users.ChangePasswordAsync(CurrentUserId, request));
}
