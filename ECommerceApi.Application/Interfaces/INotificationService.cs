using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Notification;

namespace ECommerceApi.Application.Interfaces;

public interface INotificationService
{
    Task<ServiceResult<List<NotificationDto>>> GetUserNotificationsAsync(int userId);
    Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId, int userId);
    Task<ServiceResult<bool>> MarkAllAsReadAsync(int userId);
}
