using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Register a new customer account</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request) =>
        HandleResult(await _auth.RegisterAsync(request));

    /// <summary>Login and receive JWT tokens</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) =>
        HandleResult(await _auth.LoginAsync(request));

    /// <summary>Refresh an expired access token</summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request) =>
        HandleResult(await _auth.RefreshTokenAsync(request));

    /// <summary>Logout and invalidate refresh token</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout() =>
        HandleResult(await _auth.LogoutAsync(CurrentUserId));
}
