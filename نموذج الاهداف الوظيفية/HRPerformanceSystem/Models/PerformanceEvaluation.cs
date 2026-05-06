using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRPerformanceSystem.Models;

/// <summary>
/// تقييم الأداء السنوي - يربط الموظف بأهدافه وجداراته
/// </summary>
public class PerformanceEvaluation
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    [Display(Name = "السنة")]
    public int Year { get; set; } = DateTime.Now.Year;

    [Display(Name = "المسؤول عن المراجعة")]
    public string ReviewerName { get; set; } = string.Empty;

    [Display(Name = "تاريخ التقييم")]
    public DateTime? EvaluationDate { get; set; }

    [Display(Name = "الحالة")]
    public EvaluationStatus Status { get; set; } = EvaluationStatus.Draft;

    [Display(Name = "ملاحظات عامة")]
    public string Notes { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    // Navigation
    public ICollection<ObjectiveGoal> Goals { get; set; } = new List<ObjectiveGoal>();
    public ICollection<CompetencyEvaluation> Competencies { get; set; } = new List<CompetencyEvaluation>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<SurveyResult> SurveyResults { get; set; } = new List<SurveyResult>();
    public ICollection<EvaluationComment> Comments { get; set; } = new List<EvaluationComment>();

    // Calculated Properties
    [Display(Name = "درجة الربع الأول")]
    public double Q1Score { get; set; }
    [Display(Name = "درجة الربع الثاني")]
    public double Q2Score { get; set; }
    [Display(Name = "درجة الربع الثالث")]
    public double Q3Score { get; set; }
    [Display(Name = "درجة الربع الرابع")]
    public double Q4Score { get; set; }

    [Display(Name = "التقييم العام")]
    [NotMapped]
    public double FinalScore => new[] { Q1Score, Q2Score, Q3Score, Q4Score }
        .Where(s => s > 0).DefaultIfEmpty(0).Average();

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "حالات الأشهر")]
    public string MonthsStatusJson { get; set; } = "{}";
}

public class EvaluationComment
{
    public int Id { get; set; }
    public int EvaluationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsManager { get; set; }
}

public enum EvaluationStatus
{
    [Display(Name = "مسودة")]
    Draft,
    [Display(Name = "قيد المراجعة")]
    InReview,
    [Display(Name = "معتمد")]
    Approved,
    [Display(Name = "مؤرشف")]
    Archived
}
