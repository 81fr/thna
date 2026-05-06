using HRPerformanceSystem.Data;
using HRPerformanceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HRPerformanceSystem.Services;

/// <summary>
/// خدمة سجل التدقيق - تتبع جميع العمليات في النظام
/// </summary>
public class AuditService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public AuditService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// تسجيل عملية في سجل التدقيق
    /// </summary>
    public async Task LogAsync(AuditAction action, string entityType, int entityId, string description, string? userName = null)
    {
        using var db = _factory.CreateDbContext();
        db.AuditLogs.Add(new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Description = description,
            UserName = userName ?? "النظام",
            CreatedAt = DateTime.Now
        });
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// جلب آخر السجلات
    /// </summary>
    public async Task<List<AuditLog>> GetRecentLogsAsync(int count = 50)
    {
        using var db = _factory.CreateDbContext();
        return await db.AuditLogs
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// جلب سجلات كيان محدد
    /// </summary>
    public async Task<List<AuditLog>> GetEntityLogsAsync(string entityType, int entityId)
    {
        using var db = _factory.CreateDbContext();
        return await db.AuditLogs
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}
