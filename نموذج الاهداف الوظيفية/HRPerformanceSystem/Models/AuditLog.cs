using System.ComponentModel.DataAnnotations;

namespace HRPerformanceSystem.Models;

/// <summary>
/// سجل التدقيق - تتبع جميع العمليات على النظام
/// </summary>
public class AuditLog
{
    public int Id { get; set; }

    [Display(Name = "نوع العملية")]
    public AuditAction Action { get; set; }

    [Display(Name = "الكيان المتأثر")]
    public string EntityType { get; set; } = string.Empty;

    [Display(Name = "معرف الكيان")]
    public int EntityId { get; set; }

    [Display(Name = "وصف العملية")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "المستخدم")]
    public string UserName { get; set; } = "النظام";

    [Display(Name = "البيانات القديمة")]
    public string? OldValues { get; set; }

    [Display(Name = "البيانات الجديدة")]
    public string? NewValues { get; set; }

    [Display(Name = "تاريخ العملية")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum AuditAction
{
    [Display(Name = "إنشاء")]
    Create,
    [Display(Name = "تعديل")]
    Update,
    [Display(Name = "حذف")]
    Delete,
    [Display(Name = "اعتماد")]
    Approve,
    [Display(Name = "إرسال للمراجعة")]
    Submit,
    [Display(Name = "إرجاع")]
    Return,
    [Display(Name = "تسجيل دخول")]
    Login,
    [Display(Name = "تصدير")]
    Export
}
