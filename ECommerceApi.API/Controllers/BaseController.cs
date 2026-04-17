using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? "0");

    protected IActionResult HandleResult<T>(Application.Common.ServiceResult<T> result)
    {
        if (!result.Success)
            return BadRequest(new { message = result.Message, errors = result.Errors });
        return Ok(new { success = true, data = result.Data, message = result.Message });
    }
}
