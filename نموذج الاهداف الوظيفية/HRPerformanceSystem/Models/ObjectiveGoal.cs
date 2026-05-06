using System.ComponentModel.DataAnnotations;

namespace HRPerformanceSystem.Models;

/// <summary>
/// الأهداف الوظيفية - الأهداف الاستراتيجية والتشغيلية (65% من التقييم)
/// </summary>
public class ObjectiveGoal
{
    public int Id { get; set; }

    public int EvaluationId { get; set; }
    public PerformanceEvaluation Evaluation { get; set; } = null!;

    [Display(Name = "الهدف الاستراتيجي المرتبط بالموظف")]
    public string StrategicObjective { get; set; } = string.Empty;

    [Display(Name = "المؤشرات الاستراتيجية المرتبطة بالموظف")]
    public string StrategicIndicator { get; set; } = string.Empty;

    [Display(Name = "المؤشرات التشغيلية المرتبطة بالموظف")]
    public string OperationalIndicator { get; set; } = string.Empty;

    [Display(Name = "الهدف الوظيفي")]
    public string GoalDescription { get; set; } = string.Empty;

    [Display(Name = "المستهدف السنوي")]
    public double AnnualTarget { get; set; }

    [Display(Name = "كفاءة تحقيق الهدف")]
    public string AchievementCriteria { get; set; } = string.Empty;

    [Display(Name = "ممكنات تحقيق الهدف")]
    public string Enablers { get; set; } = string.Empty;

    [Display(Name = "مصدر البيانات")]
    public string DataSource { get; set; } = string.Empty;

    [Display(Name = "الوزن النسبي")]
    public double Weight { get; set; }

    // الربع الأول
    [Display(Name = "مستهدف الربع الأول - يناير")]
    public double Q1M1Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الأول - يناير")]
    public double Q1M1Actual { get; set; }
    [Display(Name = "مستهدف الربع الأول - فبراير")]
    public double Q1M2Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الأول - فبراير")]
    public double Q1M2Actual { get; set; }
    [Display(Name = "مستهدف الربع الأول - مارس")]
    public double Q1M3Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الأول - مارس")]
    public double Q1M3Actual { get; set; }

    // الربع الثاني
    [Display(Name = "مستهدف الربع الثاني - أبريل")]
    public double Q2M1Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الثاني - أبريل")]
    public double Q2M1Actual { get; set; }
    [Display(Name = "مستهدف الربع الثاني - مايو")]
    public double Q2M2Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الثاني - مايو")]
    public double Q2M2Actual { get; set; }
    [Display(Name = "مستهدف الربع الثاني - يونيو")]
    public double Q2M3Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الثاني - يونيو")]
    public double Q2M3Actual { get; set; }

    // الربع الثالث
    [Display(Name = "مستهدف الربع الثالث - يوليو")]
    public double Q3M1Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الثالث - يوليو")]
    public double Q3M1Actual { get; set; }
    [Display(Name = "مستهدف الربع الثالث - أغسطس")]
    public double Q3M2Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الثالث - أغسطس")]
    public double Q3M2Actual { get; set; }
    [Display(Name = "مستهدف الربع الثالث - سبتمبر")]
    public double Q3M3Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الثالث - سبتمبر")]
    public double Q3M3Actual { get; set; }

    // الربع الرابع
    [Display(Name = "مستهدف الربع الرابع - أكتوبر")]
    public double Q4M1Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الرابع - أكتوبر")]
    public double Q4M1Actual { get; set; }
    [Display(Name = "مستهدف الربع الرابع - نوفمبر")]
    public double Q4M2Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الرابع - نوفمبر")]
    public double Q4M2Actual { get; set; }
    [Display(Name = "مستهدف الربع الرابع - ديسمبر")]
    public double Q4M3Target { get; set; } = 1;
    [Display(Name = "متحقق الربع الرابع - ديسمبر")]
    public double Q4M3Actual { get; set; }

    // الشواهد والمرفقات لكل شهر (مسار الملف)
    [Display(Name = "شاهد يناير")]
    public string? EvidenceM1 { get; set; }
    [Display(Name = "شاهد فبراير")]
    public string? EvidenceM2 { get; set; }
    [Display(Name = "شاهد مارس")]
    public string? EvidenceM3 { get; set; }
    [Display(Name = "شاهد أبريل")]
    public string? EvidenceM4 { get; set; }
    [Display(Name = "شاهد مايو")]
    public string? EvidenceM5 { get; set; }
    [Display(Name = "شاهد يونيو")]
    public string? EvidenceM6 { get; set; }
    [Display(Name = "شاهد يوليو")]
    public string? EvidenceM7 { get; set; }
    [Display(Name = "شاهد أغسطس")]
    public string? EvidenceM8 { get; set; }
    [Display(Name = "شاهد سبتمبر")]
    public string? EvidenceM9 { get; set; }
    [Display(Name = "شاهد أكتوبر")]
    public string? EvidenceM10 { get; set; }
    [Display(Name = "شاهد نوفمبر")]
    public string? EvidenceM11 { get; set; }
    [Display(Name = "شاهد ديسمبر")]
    public string? EvidenceM12 { get; set; }

    /// <summary>
    /// الحصول على مسار شاهد شهر معين
    /// </summary>
    public string? GetEvidence(int month) => month switch
    {
        1 => EvidenceM1, 2 => EvidenceM2, 3 => EvidenceM3,
        4 => EvidenceM4, 5 => EvidenceM5, 6 => EvidenceM6,
        7 => EvidenceM7, 8 => EvidenceM8, 9 => EvidenceM9,
        10 => EvidenceM10, 11 => EvidenceM11, 12 => EvidenceM12,
        _ => null
    };

    /// <summary>
    /// تعيين مسار شاهد لشهر معين
    /// </summary>
    public void SetEvidence(int month, string? path)
    {
        switch (month)
        {
            case 1: EvidenceM1 = path; break;
            case 2: EvidenceM2 = path; break;
            case 3: EvidenceM3 = path; break;
            case 4: EvidenceM4 = path; break;
            case 5: EvidenceM5 = path; break;
            case 6: EvidenceM6 = path; break;
            case 7: EvidenceM7 = path; break;
            case 8: EvidenceM8 = path; break;
            case 9: EvidenceM9 = path; break;
            case 10: EvidenceM10 = path; break;
            case 11: EvidenceM11 = path; break;
            case 12: EvidenceM12 = path; break;
        }
    }

    // حساب الدرجات الربعية
    public double CalcQ1Score()
    {
        var totalTarget = Q1M1Target + Q1M2Target + Q1M3Target;
        var totalActual = Q1M1Actual + Q1M2Actual + Q1M3Actual;
        return totalTarget > 0 ? (totalActual / totalTarget) * Weight : 0;
    }

    public double CalcQ2Score()
    {
        var totalTarget = Q2M1Target + Q2M2Target + Q2M3Target;
        var totalActual = Q2M1Actual + Q2M2Actual + Q2M3Actual;
        return totalTarget > 0 ? (totalActual / totalTarget) * Weight : 0;
    }

    public double CalcQ3Score()
    {
        var totalTarget = Q3M1Target + Q3M2Target + Q3M3Target;
        var totalActual = Q3M1Actual + Q3M2Actual + Q3M3Actual;
        return totalTarget > 0 ? (totalActual / totalTarget) * Weight : 0;
    }

    public double CalcQ4Score()
    {
        var totalTarget = Q4M1Target + Q4M2Target + Q4M3Target;
        var totalActual = Q4M1Actual + Q4M2Actual + Q4M3Actual;
        return totalTarget > 0 ? (totalActual / totalTarget) * Weight : 0;
    }

    public double CalculateCompletionPercentage()
    {
        double totalTarget = Q1M1Target + Q1M2Target + Q1M3Target + Q2M1Target + Q2M2Target + Q2M3Target + Q3M1Target + Q3M2Target + Q3M3Target + Q4M1Target + Q4M2Target + Q4M3Target;
        double totalActual = Q1M1Actual + Q1M2Actual + Q1M3Actual + Q2M1Actual + Q2M2Actual + Q2M3Actual + Q3M1Actual + Q3M2Actual + Q3M3Actual + Q4M1Actual + Q4M2Actual + Q4M3Actual;
        
        if (totalTarget == 0) return 0;
        var pct = (totalActual / totalTarget) * 100;
        return pct > 100 ? 100 : pct;
    }

    public int DisplayOrder { get; set; }
}
