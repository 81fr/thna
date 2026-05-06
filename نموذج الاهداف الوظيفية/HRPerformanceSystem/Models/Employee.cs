using System.ComponentModel.DataAnnotations;

namespace HRPerformanceSystem.Models;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم الموظف مطلوب")]
    [MaxLength(200, ErrorMessage = "الحد الأقصى 200 حرف")]
    [Display(Name = "اسم الموظف")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الرقم الوظيفي مطلوب")]
    [MaxLength(20, ErrorMessage = "الحد الأقصى 20 حرف")]
    [Display(Name = "الرقم الوظيفي")]
    public string EmployeeNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "المسمى الوظيفي")]
    public string JobTitle { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "الإدارة / القسم")]
    public string Department { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name = "المدير المباشر")]
    public string DirectManager { get; set; } = string.Empty;

    [Display(Name = "تاريخ التعيين")]
    public DateTime? HireDate { get; set; }

    [MaxLength(200)]
    [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
    [Display(Name = "البريد الإلكتروني")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    [Display(Name = "رقم الجوال")]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "محذوف")]
    public bool IsDeleted { get; set; }

    [Display(Name = "نوع الدوام")]
    public AttendanceType AttendanceType { get; set; } = AttendanceType.FullTime;

    [Display(Name = "نوع الميثاق")]
    public CharterType CharterType { get; set; } = CharterType.Employee;

    [Display(Name = "فعّال")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [MaxLength(450)]
    [Display(Name = "حساب المستخدم")]
    public string? UserId { get; set; }

    // Navigation
    public ICollection<PerformanceEvaluation> Evaluations { get; set; } = new List<PerformanceEvaluation>();
}

public enum AttendanceType
{
    [Display(Name = "دوام كامل")]
    FullTime,
    [Display(Name = "دوام جزئي")]
    PartTime
}

public enum CharterType
{
    [Display(Name = "ميثاق الأداء الوظيفي")]
    Employee,
    [Display(Name = "الميثاق القيادي")]
    Leader
}
