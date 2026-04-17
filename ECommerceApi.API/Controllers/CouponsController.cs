using ECommerceApi.Application.DTOs.Coupon;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/coupons")]
public class CouponsController : BaseController
{
    private readonly ICouponService _coupons;

    public CouponsController(ICouponService coupons) => _coupons = coupons;

    /// <summary>Get all coupons — Admin only</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll() =>
        HandleResult(await _coupons.GetAllAsync());

    /// <summary>Create a coupon — Admin only</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCouponRequest request) =>
        HandleResult(await _coupons.CreateAsync(request));

    /// <summary>Update a coupon — Admin only</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponRequest request) =>
        HandleResult(await _coupons.UpdateAsync(id, request));

    /// <summary>Delete a coupon — Admin only</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) =>
        HandleResult(await _coupons.DeleteAsync(id));

    /// <summary>Validate a coupon code</summary>
    [HttpPost("validate")]
    [Authorize]
    public async Task<IActionResult> Validate([FromBody] ValidateCouponRequest request) =>
        HandleResult(await _coupons.ValidateAsync(request));
}
