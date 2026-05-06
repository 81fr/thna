using HRPerformanceSystem.Models;
using HRPerformanceSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace HRPerformanceSystem.Services;

/// <summary>
/// خدمة حساب درجات الأداء الوظيفي
/// </summary>
public class CalculationService
{
    /// <summary>
    /// حساب درجة الأهداف لربع معين (65% من التقييم)
    /// المعادلة: (المتحقق / المستهدف) × الوزن النسبي لكل هدف
    /// </summary>
    public double CalculateGoalsScore(IEnumerable<ObjectiveGoal> goals, int quarter)
    {
        // For accurate handling of transfers, we calculate monthly and average them
        int startMonth = (quarter - 1) * 3 + 1;
        double m1 = CalculateGoalsMonthScore(goals, startMonth);
        double m2 = CalculateGoalsMonthScore(goals, startMonth + 1);
        double m3 = CalculateGoalsMonthScore(goals, startMonth + 2);

        // Filter out months where no goals were active
        var activeMonths = new[] { m1, m2, m3 }.Where(m => m >= 0).ToArray();
        return activeMonths.Length > 0 ? Math.Round(activeMonths.Average(), 2) : 0;
    }

    /// <summary>
    /// حساب درجة الأهداف لشهر معين
    /// </summary>
    public double CalculateGoalsMonthScore(IEnumerable<ObjectiveGoal> goals, int month)
    {
        double totalWeightedContribution = 0;
        double activeWeightSum = 0;
        const double targetTotalWeight = 65.0;

        foreach (var goal in goals)
        {
            double target = GetGoalMonthTarget(goal, month);
            double actual = GetGoalMonthActual(goal, month);

            if (target > 0)
            {
                totalWeightedContribution += (actual / target) * goal.Weight;
                activeWeightSum += goal.Weight;
            }
        }

        if (activeWeightSum == 0) return 0;

        // Scale the score to the target 65% based on active weights
        return Math.Round(totalWeightedContribution * (targetTotalWeight / activeWeightSum), 2);
    }

    private double GetGoalMonthTarget(ObjectiveGoal g, int m) => m switch { 1=>g.Q1M1Target, 2=>g.Q1M2Target, 3=>g.Q1M3Target, 4=>g.Q2M1Target, 5=>g.Q2M2Target, 6=>g.Q2M3Target, 7=>g.Q3M1Target, 8=>g.Q3M2Target, 9=>g.Q3M3Target, 10=>g.Q4M1Target, 11=>g.Q4M2Target, 12=>g.Q4M3Target, _=>0 };
    private double GetGoalMonthActual(ObjectiveGoal g, int m) => m switch { 1=>g.Q1M1Actual, 2=>g.Q1M2Actual, 3=>g.Q1M3Actual, 4=>g.Q2M1Actual, 5=>g.Q2M2Actual, 6=>g.Q2M3Actual, 7=>g.Q3M1Actual, 8=>g.Q3M2Actual, 9=>g.Q3M3Actual, 10=>g.Q4M1Actual, 11=>g.Q4M2Actual, 12=>g.Q4M3Actual, _=>0 };

    /// <summary>
    /// حساب درجة الجدارات لربع معين (35% من التقييم)
    /// سلم التقييم: 0=غير متحقق، 1=مبتدئ، 2=متمكن، 3=متميز
    /// </summary>
    public double CalculateCompetenciesScore(IEnumerable<CompetencyEvaluation> competencies, int quarter)
    {
        double total = 0;
        foreach (var comp in competencies.Where(c => c.Type != CompetencyType.Bonus))
        {
            total += quarter switch
            {
                1 => comp.CalcQ1Score(),
                2 => comp.CalcQ2Score(),
                3 => comp.CalcQ3Score(),
                4 => comp.CalcQ4Score(),
                _ => 0
            };
        }
        return Math.Round(total, 2);
    }

    /// <summary>
    /// حساب درجة الجدارات لشهر معين
    /// </summary>
    public double CalculateCompetenciesMonthScore(IEnumerable<CompetencyEvaluation> competencies, int month)
    {
        double total = 0;
        foreach (var comp in competencies.Where(c => c.Type != CompetencyType.Bonus))
        {
            total += comp.CalcMonthScore(month);
        }
        return Math.Round(total, 2);
    }

    /// <summary>
    /// حساب البونص (استثمار الفرص) - حد أقصى 5 درجات إضافية
    /// </summary>
    public double CalculateBonusScore(IEnumerable<CompetencyEvaluation> competencies, int quarter)
    {
        double total = 0;
        foreach (var comp in competencies.Where(c => c.Type == CompetencyType.Bonus))
        {
            total += quarter switch
            {
                1 => comp.CalcQ1Score(),
                2 => comp.CalcQ2Score(),
                3 => comp.CalcQ3Score(),
                4 => comp.CalcQ4Score(),
                _ => 0
            };
        }
        return Math.Min(5, Math.Round(total, 2));
    }

    public double CalculateBonusMonthScore(IEnumerable<CompetencyEvaluation> competencies, int month)
    {
        double total = 0;
        foreach (var comp in competencies.Where(c => c.Type == CompetencyType.Bonus))
        {
            total += comp.CalcMonthScore(month);
        }
        return Math.Min(5, Math.Round(total, 2));
    }

    /// <summary>
    /// حساب درجة الحضور والانصراف لربع معين
    /// </summary>
    public double CalculateAttendanceScore(IEnumerable<AttendanceRecord> records, int quarter, double baseWeight, AttendanceType type)
    {
        var quarterRecords = records.Where(r => r.Quarter == quarter);
        if (!quarterRecords.Any()) return baseWeight;

        double total = 0;
        foreach (var rec in quarterRecords)
        {
            total += type == AttendanceType.FullTime
                ? rec.CalcFullTimeScore(baseWeight / 3.0)
                : rec.CalcPartTimeScore(baseWeight / 3.0);
        }
        return Math.Max(0, Math.Round(total, 2));
    }

    /// <summary>
    /// حساب الدرجة الإجمالية لربع معين
    /// الأهداف (65%) + الجدارات (35%) + البونص (حتى 5%)
    /// الحد الأعلى: 105%
    /// </summary>
    public double CalculateQuarterTotal(PerformanceEvaluation eval, int quarter)
    {
        var goalsScore = CalculateGoalsScore(eval.Goals, quarter);
        var compScore = CalculateCompetenciesScore(eval.Competencies, quarter);
        var bonus = CalculateBonusScore(eval.Competencies, quarter);
        var attendance = CalculateAttendanceScore(eval.AttendanceRecords, quarter,
            eval.Employee?.CharterType == CharterType.Leader ? 5 : 7,
            eval.Employee?.AttendanceType ?? AttendanceType.FullTime);

        double total = goalsScore + compScore + attendance + bonus;
        return Math.Min(105, Math.Round(total, 2));
    }

    /// <summary>
    /// حساب الدرجة التوجيهية لشهر معين
    /// </summary>
    public double CalculateMonthTotal(PerformanceEvaluation eval, int month)
    {
        var goalsScore = CalculateGoalsMonthScore(eval.Goals, month);
        var compScore = CalculateCompetenciesMonthScore(eval.Competencies, month);
        var bonus = CalculateBonusMonthScore(eval.Competencies, month);
        
        // جلب الحضور لهذا الشهر، مع تحديد وزنه الشهري (ثلث وزن الربع)
        double baseWeight = eval.Employee?.CharterType == CharterType.Leader ? 5 : 7;
        var monthRecords = eval.AttendanceRecords.Where(r => r.Month == month);
        double attendance = baseWeight / 3.0; // إذا لم يكن هناك غياب، يأخذ العلامة كاملة عن هذا الشهر
        if (monthRecords.Any())
        {
            var type = eval.Employee?.AttendanceType ?? AttendanceType.FullTime;
            var rec = monthRecords.First();
            attendance = type == AttendanceType.FullTime
                ? rec.CalcFullTimeScore(attendance)
                : rec.CalcPartTimeScore(attendance);
        }

        double total = goalsScore + compScore + attendance + bonus;
        return Math.Min(105, Math.Round(total, 2));
    }

    public double CalculateGoalsTotal(PerformanceEvaluation eval)
    {
        double total = 0;
        foreach (var goal in eval.Goals)
        {
            total += (goal.Weight * goal.CalculateCompletionPercentage() / 100);
        }
        return Math.Min(65, Math.Round(total, 2));
    }

    public double CalculateCompetenciesTotal(PerformanceEvaluation eval)
    {
        double total = 0;
        foreach (var comp in eval.Competencies.Where(c => c.Type != CompetencyType.Bonus))
        {
            total += (comp.Weight * comp.CalculateAverageRating() / 5);
        }
        return Math.Min(35, Math.Round(total, 2));
    }

    /// <summary>
    /// حساب التقييم النهائي السنوي = متوسط الأرباع الأربعة
    /// </summary>
    public double CalculateFinalScore(PerformanceEvaluation eval)
    {
        var quarters = new[] {
            CalculateQuarterTotal(eval, 1),
            CalculateQuarterTotal(eval, 2),
            CalculateQuarterTotal(eval, 3),
            CalculateQuarterTotal(eval, 4)
        };

        var activeQuarters = quarters.Where(q => q > 0).ToArray();
        return activeQuarters.Length > 0 ? Math.Round(activeQuarters.Average(), 2) : 0;
    }

    /// <summary>
    /// تحديد التصنيف بناءً على الدرجة
    /// </summary>
    public string GetRating(double score)
    {
        return score switch
        {
            >= 95 => "متميز ⭐",
            >= 85 => "ممتاز",
            >= 75 => "جيد جداً",
            >= 65 => "جيد",
            >= 50 => "مقبول",
            _ => "غير مرضي"
        };
    }

    public string GetRatingColor(double score)
    {
        return score switch
        {
            >= 95 => "#10b981",
            >= 85 => "#3b82f6",
            >= 75 => "#8b5cf6",
            >= 65 => "#f59e0b",
            >= 50 => "#f97316",
            _ => "#ef4444"
        };
    }
}
