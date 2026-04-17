using ECommerceApi.Application.DTOs.Order;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/orders")]
[Authorize]
public class OrdersController : BaseController
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders) => _orders = orders;

    /// <summary>Get the current user's orders</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        HandleResult(await _orders.GetUserOrdersAsync(CurrentUserId, page, pageSize));

    /// <summary>Get a specific order belonging to the current user</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) =>
        HandleResult(await _orders.GetByIdAsync(id, CurrentUserId));

    /// <summary>Place an order from the current cart</summary>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request) =>
        HandleResult(await _orders.PlaceOrderAsync(CurrentUserId, request));

    /// <summary>Cancel a pending order</summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id) =>
        HandleResult(await _orders.CancelOrderAsync(id, CurrentUserId));
}

[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : BaseController
{
    private readonly IOrderService _orders;

    public AdminOrdersController(IOrderService orders) => _orders = orders;

    /// <summary>Get all orders — Admin only</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderFilterRequest filter) =>
        HandleResult(await _orders.GetAllOrdersAsync(filter));

    /// <summary>Get any order by ID — Admin only</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) =>
        HandleResult(await _orders.GetByIdAsync(id));

    /// <summary>Update order status — Admin only</summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request) =>
        HandleResult(await _orders.UpdateStatusAsync(id, request));
}
