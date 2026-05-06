using HRPerformanceSystem.Data;
using HRPerformanceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HRPerformanceSystem.Services;

/// <summary>
/// خدمة الإشعارات - إدارة إشعارات النظام
/// </summary>
public class NotificationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public NotificationService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// إنشاء إشعار جديد
    /// </summary>
    public async Task CreateAsync(string title, string message, NotificationType type, string? link = null, string? targetUserId = null)
    {
        using var db = _factory.CreateDbContext();
        db.Notifications.Add(new Notification
        {
            Title = title,
            Message = message,
            Type = type,
            Link = link,
            TargetUserId = targetUserId,
            CreatedAt = DateTime.Now
        });
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// جلب الإشعارات غير المقروءة
    /// </summary>
    public async Task<List<Notification>> GetUnreadAsync(int count = 20)
    {
        using var db = _factory.CreateDbContext();
        return await db.Notifications
            .Where(n => !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// جلب جميع الإشعارات
    /// </summary>
    public async Task<List<Notification>> GetAllAsync(int count = 50)
    {
        using var db = _factory.CreateDbContext();
        return await db.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// تحديد إشعار كمقروء
    /// </summary>
    public async Task MarkAsReadAsync(int id)
    {
        using var db = _factory.CreateDbContext();
        var notification = await db.Notifications.FindAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            await db.SaveChangesAsync();
        }
    }

    /// <summary>
    /// تحديد جميع الإشعارات كمقروءة
    /// </summary>
    public async Task MarkAllAsReadAsync()
    {
        using var db = _factory.CreateDbContext();
        var unread = await db.Notifications.Where(n => !n.IsRead).ToListAsync();
        foreach (var n in unread) n.IsRead = true;
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// عدد الإشعارات غير المقروءة
    /// </summary>
    public async Task<int> GetUnreadCountAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.Notifications.CountAsync(n => !n.IsRead);
    }
}
