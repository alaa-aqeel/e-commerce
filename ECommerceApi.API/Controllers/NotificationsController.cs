using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[Route("api/notifications")]
[Authorize]
public class NotificationsController : BaseController
{
    private readonly INotificationService _notifications;

    public NotificationsController(INotificationService notifications) => _notifications = notifications;

    /// <summary>Get the current user's notifications</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        HandleResult(await _notifications.GetUserNotificationsAsync(CurrentUserId));

    /// <summary>Mark a notification as read</summary>
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id) =>
        HandleResult(await _notifications.MarkAsReadAsync(id, CurrentUserId));

    /// <summary>Mark all notifications as read</summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead() =>
        HandleResult(await _notifications.MarkAllAsReadAsync(CurrentUserId));
}
