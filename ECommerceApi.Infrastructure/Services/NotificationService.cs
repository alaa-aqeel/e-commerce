using AutoMapper;
using ECommerceApi.Application.Common;
using ECommerceApi.Application.DTOs.Notification;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public NotificationService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceResult<List<NotificationDto>>> GetUserNotificationsAsync(int userId)
    {
        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return ServiceResult<List<NotificationDto>>.Ok(_mapper.Map<List<NotificationDto>>(notifications));
    }

    public async Task<ServiceResult<bool>> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null) return ServiceResult<bool>.Fail("Notification not found.");

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        notifications.ForEach(n => n.IsRead = true);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
