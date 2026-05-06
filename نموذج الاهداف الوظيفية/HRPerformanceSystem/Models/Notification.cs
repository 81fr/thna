using System.ComponentModel.DataAnnotations;

namespace HRPerformanceSystem.Models;

/// <summary>
/// نظام الإشعارات والتنبيهات
/// </summary>
public class Notification
{
    public int Id { get; set; }

    [Display(Name = "العنوان")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "المحتوى")]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "نوع الإشعار")]
    public NotificationType Type { get; set; }

    [Display(Name = "الرابط")]
    public string? Link { get; set; }

    [Display(Name = "مقروء")]
    public bool IsRead { get; set; }

    [Display(Name = "تاريخ الإنشاء")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// معرف المستخدم المستهدف (فارغ = للجميع)
    /// </summary>
    public string? TargetUserId { get; set; }
}

public enum NotificationType
{
    [Display(Name = "معلومات")]
    Info,
    [Display(Name = "تنبيه")]
    Warning,
    [Display(Name = "نجاح")]
    Success,
    [Display(Name = "طلب مراجعة")]
    ReviewRequest,
    [Display(Name = "تم الاعتماد")]
    Approved,
    [Display(Name = "تذكير")]
    Reminder
}
