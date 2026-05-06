using System.ComponentModel.DataAnnotations;

namespace HRPerformanceSystem.Models;

/// <summary>
/// الجدارات الوظيفية (35% من التقييم)
/// تشمل: الأساسية، المهارية، الابتكارية
/// </summary>
public class CompetencyEvaluation
{
    public int Id { get; set; }

    public int EvaluationId { get; set; }
    public PerformanceEvaluation Evaluation { get; set; } = null!;

    [Display(Name = "نوع الجدارة")]
    public CompetencyType Type { get; set; }

    [Display(Name = "اسم الجدارة")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "الوزن النسبي")]
    public double Weight { get; set; }

    [Display(Name = "الوصف")]
    public string Description { get; set; } = string.Empty;

    // الربع الأول
    [Display(Name = "تقييم يناير")]
    public int Q1M1Rating { get; set; }
    [Display(Name = "تقييم فبراير")]
    public int Q1M2Rating { get; set; }
    [Display(Name = "تقييم مارس")]
    public int Q1M3Rating { get; set; }

    // الربع الثاني
    [Display(Name = "تقييم أبريل")]
    public int Q2M1Rating { get; set; }
    [Display(Name = "تقييم مايو")]
    public int Q2M2Rating { get; set; }
    [Display(Name = "تقييم يونيو")]
    public int Q2M3Rating { get; set; }

    // الربع الثالث
    [Display(Name = "تقييم يوليو")]
    public int Q3M1Rating { get; set; }
    [Display(Name = "تقييم أغسطس")]
    public int Q3M2Rating { get; set; }
    [Display(Name = "تقييم سبتمبر")]
    public int Q3M3Rating { get; set; }

    // الربع الرابع
    [Display(Name = "تقييم أكتوبر")]
    public int Q4M1Rating { get; set; }
    [Display(Name = "تقييم نوفمبر")]
    public int Q4M2Rating { get; set; }
    [Display(Name = "تقييم ديسمبر")]
    public int Q4M3Rating { get; set; }

    // حساب الدرجة الربعية = (مجموع تقييمات الأشهر الثلاثة / 9) × الوزن
    public double CalcQ1Score() => ((Q1M1Rating + Q1M2Rating + Q1M3Rating) / 9.0) * Weight;
    public double CalcQ2Score() => ((Q2M1Rating + Q2M2Rating + Q2M3Rating) / 9.0) * Weight;
    public double CalcQ3Score() => ((Q3M1Rating + Q3M2Rating + Q3M3Rating) / 9.0) * Weight;
    public double CalcQ4Score() => ((Q4M1Rating + Q4M2Rating + Q4M3Rating) / 9.0) * Weight;

    // دالة لحساب درجة شهر محدد
    public double CalcMonthScore(int month)
    {
        int rating = month switch
        {
            1 => Q1M1Rating, 2 => Q1M2Rating, 3 => Q1M3Rating,
            4 => Q2M1Rating, 5 => Q2M2Rating, 6 => Q2M3Rating,
            7 => Q3M1Rating, 8 => Q3M2Rating, 9 => Q3M3Rating,
            10 => Q4M1Rating, 11 => Q4M2Rating, 12 => Q4M3Rating,
            _ => 0
        };
        return (rating / 3.0) * Weight;
    }

    public double CalculateAverageRating()
    {
        var ratings = new[] { Q1M1Rating, Q1M2Rating, Q1M3Rating, Q2M1Rating, Q2M2Rating, Q2M3Rating, Q3M1Rating, Q3M2Rating, Q3M3Rating, Q4M1Rating, Q4M2Rating, Q4M3Rating };
        var active = ratings.Where(r => r > 0).ToList();
        return active.Any() ? active.Average() : 0;
    }

    public int DisplayOrder { get; set; }
}

public enum CompetencyType
{
    [Display(Name = "جدارات أساسية")]
    Core,
    [Display(Name = "جدارات مهارية")]
    Skills,
    [Display(Name = "جدارات ابتكارية")]
    Innovation,
    [Display(Name = "إضافي")]
    Bonus
}

/// <summary>
/// سجل الحضور والانصراف الشهري
/// </summary>
public class AttendanceRecord
{
    public int Id { get; set; }

    public int EvaluationId { get; set; }
    public PerformanceEvaluation Evaluation { get; set; } = null!;

    [Display(Name = "الشهر")]
    public int Month { get; set; }

    [Display(Name = "الربع")]
    public int Quarter { get; set; }

    // الدوام الكامل
    [Display(Name = "دقائق التأخير - الدوام الكامل")]
    public int FullTimeLateMinutes { get; set; }

    [Display(Name = "أيام الغياب - الدوام الكامل")]
    public int FullTimeAbsentDays { get; set; }

    // الدوام الجزئي
    [Display(Name = "دقائق التأخير - الدوام الجزئي")]
    public int PartTimeLateMinutes { get; set; }

    [Display(Name = "أيام الغياب - الدوام الجزئي")]
    public int PartTimeAbsentDays { get; set; }

    /// <summary>
    /// حساب درجة الحضور - الدوام الكامل
    /// الوزن الأساسي 7 (قيادي 5)
    /// كل 120 دقيقة تأخير = خصم درجة
    /// كل يوم غياب بدون عذر = خصم 4 درجات
    /// </summary>
    public double CalcFullTimeScore(double baseWeight)
    {
        double deduction = ((FullTimeLateMinutes + (FullTimeAbsentDays * 8 * 60)) / 30.0) * 0.25;
        return Math.Max(0, baseWeight - deduction);
    }

    /// <summary>
    /// حساب درجة الحضور - الدوام الجزئي
    /// كل 60 دقيقة تأخير = خصم درجة
    /// </summary>
    public double CalcPartTimeScore(double baseWeight)
    {
        double deduction = ((PartTimeLateMinutes / 30.0)) * 0.5 - (PartTimeAbsentDays * 2.8);
        return Math.Max(0, baseWeight - deduction);
    }
}

/// <summary>
/// نتائج الاستبيانات (للميثاق القيادي)
/// </summary>
public class SurveyResult
{
    public int Id { get; set; }

    public int EvaluationId { get; set; }
    public PerformanceEvaluation Evaluation { get; set; } = null!;

    [Display(Name = "نوع الاستبيان")]
    public SurveyType Type { get; set; }

    [Display(Name = "السؤال")]
    public string Question { get; set; } = string.Empty;

    [Display(Name = "الإجابة")]
    public bool? Answer { get; set; }

    public int DisplayOrder { get; set; }
}

public enum SurveyType
{
    [Display(Name = "استبيان استراتيجي")]
    Strategic,
    [Display(Name = "استبيان تشغيلي")]
    Operational,
    [Display(Name = "استبيان بدون موظفين")]
    NoEmployees
}
