using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : BaseController
{
    private readonly IUserService _users;

    public UsersController(IUserService users) => _users = users;

    /// <summary>Get all users (paginated) — Admin only</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        HandleResult(await _users.GetAllUsersAsync(page, pageSize));

    /// <summary>Get user by ID — Admin only</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) =>
        HandleResult(await _users.GetUserByIdAsync(id));

    /// <summary>Delete user — Admin only</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        HandleResult(await _users.DeleteUserAsync(id));
}
