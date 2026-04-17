using ECommerceApi.Application.DTOs.Address;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/addresses")]
[Authorize]
public class AddressesController : BaseController
{
    private readonly IAddressService _addresses;

    public AddressesController(IAddressService addresses) => _addresses = addresses;

    /// <summary>Get the current user's saved addresses</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        HandleResult(await _addresses.GetUserAddressesAsync(CurrentUserId));

    /// <summary>Add a new address</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressRequest request) =>
        HandleResult(await _addresses.CreateAsync(CurrentUserId, request));

    /// <summary>Update an address</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressRequest request) =>
        HandleResult(await _addresses.UpdateAsync(id, CurrentUserId, request));

    /// <summary>Delete an address</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        HandleResult(await _addresses.DeleteAsync(id, CurrentUserId));
}
