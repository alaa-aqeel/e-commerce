using ECommerceApi.Application.DTOs.Cart;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/cart")]
[Authorize]
public class CartController : BaseController
{
    private readonly ICartService _cart;

    public CartController(ICartService cart) => _cart = cart;

    /// <summary>Get the current user's cart</summary>
    [HttpGet]
    public async Task<IActionResult> GetCart() =>
        HandleResult(await _cart.GetCartAsync(CurrentUserId));

    /// <summary>Add an item to the cart</summary>
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request) =>
        HandleResult(await _cart.AddItemAsync(CurrentUserId, request));

    /// <summary>Update a cart item's quantity</summary>
    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateItem(int itemId, [FromBody] UpdateCartItemRequest request) =>
        HandleResult(await _cart.UpdateItemAsync(CurrentUserId, itemId, request));

    /// <summary>Remove an item from the cart</summary>
    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int itemId) =>
        HandleResult(await _cart.RemoveItemAsync(CurrentUserId, itemId));

    /// <summary>Clear the entire cart</summary>
    [HttpDelete]
    public async Task<IActionResult> ClearCart() =>
        HandleResult(await _cart.ClearCartAsync(CurrentUserId));
}
